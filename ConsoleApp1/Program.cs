using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            //var statementDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Tuesday).ToString("yyyyMMdd");
            var statementDate = "20171129";

            IAmazonS3 s3client;
            s3client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);


            FTPClient client = new FTPClient("ftp://ftp.unifiedgrocers.com/", "newseasons", "TrKy5jgS", true);
            List<string> files = client.DirectoryListing("INBOUND");
            List<string> filteredFileList = files.Where(fileList => fileList.Contains(statementDate)).ToList();

            //.Where(fileName => fileName.Contains(statementDate)); 
            foreach (string s in filteredFileList)
            {
                Console.WriteLine(s);
            }

            foreach (string s in filteredFileList)
            {
                client.Download("INBOUND", s, "C:\\Users\\jason.lee\\source\\repos\\ConsoleApp1\\" + s);
                // Stream to S3
                // S3_KEY is name of file we want to upload
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = "nsm.sh-integrations",
                    InputStream = client
                    //FilePath = filePath
                };
                PutObjectResponse response2 = s3client.PutObject(request); 
                request.WithKey(S3_KEY);
                request.WithInputStream(ms);
                s3client.PutObject(request);
            }
            //client.Download("INBOUND/SHIP_STMT_45883_20171129_002655.DAT", "C:\\Users\\jason.lee\\source\\repos\\ConsoleApp1\\SHIP_STMT_45883_20171129_002655.DAT");

            Console.WriteLine(statementDate);

            HelloWorld.Hello("Hola!!!");

        }
    }

    class HelloWorld
    {

        internal static void Hello(string greeting)
        {
            Console.WriteLine(greeting);
            // Keep window open.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();         
        }

    }

    public class UploadToS3
    {
        private string bucket;
        private string keyName;
        private string fileName;

    }

    public class FTPClient
    {
        private string user;
        private string pass;
        private string host;

        public FTPClient(string remoteHost, string remoteUser, string remotePass, bool debug)
        {
            host = remoteHost;
            user = remoteUser;
            pass = remotePass;
        }

        public List<string> DirectoryListing(string folder)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + folder);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(user, pass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> result = new List<string>();
             
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();
            return result;
        }

        public void Download(string folder, string file, string destination)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(host + folder + "/" + file);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(user, pass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            StreamWriter writer = new StreamWriter(destination);
            writer.Write(reader.ReadToEnd());

            writer.Close();
            reader.Close();
            response.Close();
        }

    }
}
