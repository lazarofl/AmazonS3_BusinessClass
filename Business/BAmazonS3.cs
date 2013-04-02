using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Amazon.S3.Model;
using Amazon.S3;
using System.Linq;

namespace Business
{
    public class BAmazonS3 : IBAmazonS3
    {
        private readonly string _awsAccessKey;
        private readonly string _awsSecretAccessKey;
        private readonly string _bucketname;

        public BAmazonS3()
        {
            _awsAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            _awsSecretAccessKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            _bucketname = ConfigurationManager.AppSettings["bucketname"];
        }

        public BAmazonS3(string pawsAccessKey, string pawsSecretAccessKey, string pbucketname)
        {
            _awsAccessKey = pawsAccessKey;
            _awsSecretAccessKey = pawsSecretAccessKey;
            _bucketname = pbucketname;
        }

        public void SaveObject(Stream pObject, string keyname, bool octetstream = false)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    PutObjectRequest request = new PutObjectRequest();
                    if (octetstream)
                        request.AddHeader("Content-Disposition", "attachment");
                    request.WithBucketName(_bucketname).WithKey(keyname).WithInputStream(pObject);

                    using (client.PutObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
            }
        }

        public void SaveTempObject(Stream pObject, string filename)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    var request = new PutObjectRequest();
                    request.WithBucketName(_bucketname).WithKey(string.Format(@"temp/{0}", filename)).WithInputStream(pObject);

                    using (client.PutObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
            }
        }

        public void DeleteTempObject(string keyname)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest();

                request.WithBucketName(_bucketname).WithKey(string.Format(@"temp/{0}", keyname));

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (client.DeleteObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message));
            }
        }


        public void MoveObject(string originalkey, string destinationkey)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    var request = new CopyObjectRequest();
                    request.DestinationBucket = _bucketname;
                    request.WithSourceBucket(_bucketname).WithSourceKey(originalkey).WithDestinationKey(destinationkey);

                    using (client.CopyObject(request)) { }
                    this.DeleteObject(originalkey);
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
            }
        }

        public void CreateFolder(string folderpath)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    PutObjectRequest request = new PutObjectRequest();
                    request.
                        WithBucketName(_bucketname).
                        WithKey(folderpath.EndsWith(@"/") ? folderpath : string.Format(@"{0}/", folderpath)).
                        WithContentBody("");
                    using (client.PutObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
            }
        }

        public Stream GetObject(string keyname)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest().WithBucketName(_bucketname).WithKey(keyname);

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        return response.ResponseStream;
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message));
            }
        }

        public Stream GetTempObject(string keyname)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest().WithBucketName(_bucketname).WithKey(string.Format(@"temp/{0}", keyname));

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        return response.ResponseStream;
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message));
            }
        }

        public long GetObjectSize(string keyname)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest().WithBucketName(_bucketname).WithKey(keyname);

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        return response.ContentLength;
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message));
            }
        }
        public long GetTempObjectSize(string keyname)
        {
            return this.GetObjectSize(string.Format(@"temp/{0}", keyname));
        }

        public void DeleteObject(string keyname)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest();

                request.WithBucketName(_bucketname).WithKey(keyname);

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (client.DeleteObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message));
            }
        }

        public void DeleteObjects(string[] keynames)
        {
            try
            {
                DeleteObjectsRequest request = new DeleteObjectsRequest();

                request.WithBucketName(_bucketname).WithKeys(keynames.Select(x => new KeyVersion(x)).ToArray());
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    using (client.DeleteObjects(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message));
            }
        }

        public IEnumerable<Stream> ListObjects(string folder, int? max = null)
        {
            try
            {
                return ListNames(folder, max).Select(x => GetObject(x));
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message));
            }
        }

        public IEnumerable<string> ListNames(string folder, int? max = null)
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest { BucketName = _bucketname };

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(_awsAccessKey, _awsSecretAccessKey))
                {
                    request.WithPrefix(folder);
                    if (max.HasValue)
                        request.WithMaxKeys(max.Value);

                    using (ListObjectsResponse response = client.ListObjects(request))
                    {
                        return response.S3Objects.Select(x => x.Key);
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Please check the provided AWS Credentials. If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                throw new Exception(string.Format("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message));
            }
        }

        /// <summary>
        /// Gets the URL to access a file.
        /// </summary>
        /// <param name="keyname">full key name.</param>
        /// <returns></returns>
        public string GetUrl(string keyname)
        {
            return string.Concat(ConfigurationManager.AppSettings["bucketwebsite"], keyname);
        }
        public string GetTempUrl(string keyname)
        {
            return string.Concat(ConfigurationManager.AppSettings["bucketwebsite"], string.Format(@"temp/{0}", keyname));
        }
    }
}
