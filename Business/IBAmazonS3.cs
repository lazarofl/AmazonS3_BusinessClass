using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Business
{
    public interface IBAmazonS3
    {
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
    }
}
