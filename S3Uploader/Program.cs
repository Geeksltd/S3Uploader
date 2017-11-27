using System;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;

class Program
{
    static TransferUtility Util;
    static string[] Args;
    static DirectoryInfo UploadAndDeleteSource;

    static string Param(string name)
    {
        var result = Args.FirstOrDefault(x => x.StartsWith($"/{name}:"))?.TrimStart($"/{name}:");
        if (result.IsEmpty()) throw new Exception("Expected parameter missing: /" + name + ":???");

        return result;
    }

    static void Main(string[] args)
    {
        Args = args;
        if (Args.None())
        {
            Console.WriteLine("Specify: /key:... /secret:.... /bucket:.... /uploadAndDelete:{path}");
            return;
        }

        using (var client = new AmazonS3Client(Param("key"), Param("secret"), Amazon.RegionEndpoint.EUWest1))
        using (Util = new TransferUtility(client)) Upload();
    }

    static void Upload()
    {
        UploadAndDeleteSource = Param("uploadAndDelete").AsDirectory();

        var toUpload = UploadAndDeleteSource.GetFiles(includeSubDirectories: true)
           .Select(x => x.AsFile()).OrderBy(x => x.CreationTimeUtc).ToArray();

        Console.WriteLine("Found " + toUpload.Length + " files to move to S3.");

        toUpload.AsParallel().ForAll(UploadFile);

        Console.WriteLine("Moved " + toUpload.Length + " files successfully.");
    }

    static void UploadFile(FileInfo file)
    {
        var key = file.Directory.FullName.TrimStart(UploadAndDeleteSource.FullName)
              .TrimStart("\\").TrimEnd("\\").WithSuffix("/")
              + file.NameWithoutExtension();

        Console.Write("Moving key:" + key + "   file:" + file.FullName.TrimStart(UploadAndDeleteSource.FullName) + "...");

        Util.Upload(file.FullName, Param("bucket"), key);
        file.Delete();
    }
}
