#Amazon S3 Business Class
###.NET class that facilitates the use of the Amazon AWS SDK with S3 service

##Current implementations

```csharp
void SaveTempObject(Stream pObject, string filename);
void DeleteTempObject(string keyname);
void SaveObject(Stream pObject, string keyname, bool octetstream = false);
Stream GetObject(string keyname);
Stream GetTempObject(string keyname);
void DeleteObject(string keyname);
void DeleteObjects(string[] keynames);
IEnumerable<Stream> ListObjects(string folder, int? max = null);
IEnumerable<string> ListNames(string folder, int? max = null);
void MoveObject(string originalkey, string destinationkey);
void CreateFolder(string folderpath);
string GetUrl(string keyname);
string GetTempUrl(string keyname);
long GetObjectSize(string keyname);
long GetTempObjectSize(string keyname);
```

#Dependencies

http://nuget.org/packages/AWSSDK

AWS SDK for .NET

```
PM> Install-Package AWSSDK
```