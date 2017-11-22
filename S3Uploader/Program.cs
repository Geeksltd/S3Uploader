using System;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Transfer;

class Program
{
    static TransferUtility Util;
    static string[] Args;

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
        var source = Param("uploadAndDelete").AsDirectory();

        var toUpload = source.GetFiles(includeSubDirectories: true)
           .Select(x => x.AsFile()).OrderBy(x => x.CreationTimeUtc).ToArray();

        Console.WriteLine("Found " + toUpload.Length + " files to move to S3.");

        foreach (var file in toUpload)
        {
            var key = file.Directory.FullName.TrimStart(source.FullName)
                .TrimStart("\\").TrimEnd("\\").WithSuffix("/")
                + file.NameWithoutExtension();

            Console.Write("Moving key:" + key + "   file:" + file.FullName.TrimStart(source.FullName) + "...");

            Util.Upload(file.FullName, Param("bucket"), key);
            file.Delete();

            Console.WriteLine("Done");
        }

        Console.WriteLine("Moved " + toUpload.Length + " files successfully.");
    }
}
