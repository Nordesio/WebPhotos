using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace RecBot
{
    internal class RecognizeProcess
    {
        private string link;
        private string name;

        private static Random rnd = new Random();
        private static object locker = new object();
        private static int limit = 1;
        private static int count = 0;
        private static bool IsResourceFree
        {
            get
            {
                lock (locker)
                {
                    if (count < limit)
                    {
                        count++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            set
            {
                lock (locker)
                {
                    count--;
                }
            }
        }

        internal RecognizeProcess(string link)
        {
            this.link = link;
            name = Guid.NewGuid().ToString();
        }

        internal string RecognizeText()
        {
            string answer = "";

            try
            {
                if (link.StartsWith("data:image/jpg;base64"))
                    link = SaveBase64Image();



                StartRecognizer();

                answer = GetRecognized();

                //DeleteDirectory();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return answer;
        }

        private string SaveBase64Image()
        {
            string data = link.Split(',')[1];
            string newLink = $@"C:\Users\Public\Downloads\Recognizer\tmp\{name}.jpg";

            byte[] bytes = Convert.FromBase64String(data);

            using (FileStream fstream = new FileStream(newLink, FileMode.OpenOrCreate))
                fstream.Write(bytes, 0, bytes.Length);

            return newLink;
        }

        private void StartRecognizer()
        {
           
            while (!IsResourceFree)
                Thread.Sleep(rnd.Next(1000, 10000));

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();
#if DEBUG
            process.StartInfo.FileName = @"C:\Users\Влад\AppData\Local\Programs\Python\Python310\python.exe";
#else
            process.StartInfo.FileName = @"C:\Users\Админ\AppData\Local\Programs\Python\Python37\python.exe";
#endif
            process.StartInfo.Arguments = $"TesseractRecognizer.py \"{link}\" \"{name}\"";
            //process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            IsResourceFree = true;
        }

        private string GetRecognized()
        {
            string answer = "";

            List<string> strings = new List<string>();

            using (StreamReader reader = new StreamReader($@"C:\Users\Public\Downloads\Recognizer\{name}\texts.txt"))
                strings = reader.ReadToEnd().Split('\n').ToList();

            strings.RemoveAll(req => string.IsNullOrWhiteSpace(req));

            foreach (string s in strings)
                answer += s + '\n';

            return answer;
        }

        private void DeleteDirectory()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo($@"C:\Users\Public\Downloads\Recognizer\{name}");
                dir.Delete(true);
            }
            catch (Exception ex)
            { }
        }
    }
}
