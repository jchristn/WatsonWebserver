using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WatsonWebserver;

namespace Test.Parameters
{
    class Program
    {
        static string _Hostname = "127.0.0.1";
        static int _Port = 8081;
        static Server _Server = null;

        static void Main(string[] args)
        {
            _Server = new Server(_Hostname, _Port, false);
            _Server.Start();

            StaticPostTest1();
            StaticPostTest2();
            StaticPostTest3();

            PostTest1();
            PostTest2();
            PostTest3();

            StaticGetTest1();
            StaticGetTest2();

            GetTest1();
            GetTest2();
            GetTest3();

            NameGetTest1();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void NameGetTest1()
        {
            string path = "/MyStaticApi/GetTest1000"; // GetTest1000 -> GetTestNoNumber

            try
            {
                new WebClient().DownloadString(GetUrl(path));
                Console.WriteLine($"Passed - {path}");
            }
            catch (WebException wex)
            {
                if (wex.Message.Contains("404"))
                    Console.WriteLine($"Failed - {path}");
                else
                    throw;
            }
        }

        private static void StaticGetTest1()
        {
            string path = "/MyStaticApi/GetTest1?x=2&y=5";

            var strResponse = new WebClient().DownloadString(GetUrl(path));

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == 10 && apiResult.Message == null)
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void GetTest1()
        {
            string path = "/MyApi/GetTest1?x=2&y=5";

            var strResponse = new WebClient().DownloadString(GetUrl(path));

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == 10 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void StaticGetTest2()
        {
            string path = "/MyStaticApi/GetTest2?x=2&y=5";

            var strResponse = new WebClient().DownloadString(GetUrl(path));

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == 10 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void GetTest2()
        {
            string path = "/MyApi/GetTest2?x=2&y=5";

            var strResponse = new WebClient().DownloadString(GetUrl(path));

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == 10 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void GetTest3()
        {
            string path = "/MyApi/GetTest2"; // GetTest2 - testing default parameters

            var strResponse = new WebClient().DownloadString(GetUrl(path));

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == 0 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void PostTest1()
        {
            int toSend = 5;

            string path = "/MyApi/PostTest1";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * 2 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void StaticPostTest1()
        {
            int toSend = 5;

            string path = "/MyStaticApi/PostTest1";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * 2)
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void PostTest2()
        {
            int toSend = 5;

            string path = "/MyApi/PostTest2";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * 2 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void StaticPostTest2()
        {
            int toSend = 5;

            string path = "/MyStaticApi/PostTest2";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * 2 && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void PostTest3()
        {
            int toSend = 5;
            int multiplier = 10;

            string path = $"/MyApi/PostTest3?multiplier={multiplier}";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * multiplier && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static void StaticPostTest3()
        {
            int toSend = 5;
            int multiplier = 10;

            string path = $"/MyStaticApi/PostTest3?multiplier={multiplier}";

            var strResponse = GetPostRequestResult(path, new MyClass() { X = toSend });

            var apiResult = JsonConvert.DeserializeObject<MyClass>(strResponse);
            if (apiResult.X == toSend * multiplier && apiResult.Message.Contains(_Hostname))
                Console.WriteLine($"Passed - {path}");
            else
                Console.WriteLine($"Failed - {path}");
        }

        private static string GetPostRequestResult(string path, object postData)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetUrl(path));
            req.Method = "POST";

            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(postData);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)req.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        private static string GetUrl(string path)
        {
            return $"http://{_Hostname}:{_Port}{path}";
        }
    }
}
