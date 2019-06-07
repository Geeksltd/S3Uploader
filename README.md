# S3Uploader
Simple utility to upload a whole folder to Amazon S3 with command line.

To run the command line, use the following pattern:
```
S3Uploader.exe /key:{KEY} /secret:{SECRET} /bucket:{BUCKET} /uploadAndDelete:{PATH}
```

- `{KEY}` is the Access Key Id of the IAM user.
- `{SECRET}` is the Secret Acces Key for the same UAM user.
- `{BUCKET}` is the plain name of the S3 Bucket to upload to.
- `{PATH}` is the folder path of which contents will be directly uploaded to the bucket.

### Optional arguments

You can optionally specify the following additional arguments:

- `/lowercase` will convert the file keys to lower-case
- `/uppercase` will convert the file keys to UPPER-CASE
- `/noExtension` will remove the file extensions from the S3 object keys
