 using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;
using EP.Ner;

namespace EP.DemoServer
{
    /// <summary>
    /// Пример обработчика, которых работает под MONO в других операционных системах
    /// (Linux, Mac, Android)
    /// Работает как в пакетном режиме, так и в режиме обработки TCP-сервера.
    /// На входе получает текст UTF-8, на выходе формирует XML.
    /// Проект можно использовать как пример для создания своих обработчиков.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int port = Properties.Settings.Default.Port;
            string addr = Properties.Settings.Default.Address;            
            for (int i = 0; i < args.Length; i++)
                if (args[i] == "-port" && i + 1 < args.Length)
                    int.TryParse(args[i + 1], out port);
                else if (args[i] == "-address" && i + 1 < args.Length)
                    addr = args[i + 1];                

            Console.Write("Initializing SDK Pullenti version {0} ... ", ProcessorService.Version);
            Sdk.Initialize();
            Console.WriteLine(" OK ");

                // режим TCP-сервера
                Console.Write("Start server (address: {0}, port: {1}) ... ", string.IsNullOrEmpty(addr) ? "*" : addr, port);

                m_Listener = new HttpListener();
                try
                {
                    if (!string.IsNullOrEmpty(addr))
                        m_Listener.Prefixes.Add(string.Format("http://{0}:{1}/", addr, port));
                    else
                        m_Listener.Prefixes.Add(string.Format("http://*:{0}/", port));
                    m_Listener.Start();

                    m_Thread = new Thread(_processTCP);
                    m_Thread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" <- ERROR: {0} ", ex.Message);
                    return;
                }

                Console.WriteLine(" OK\r\nReady (to exit server press any key) ... ");

                // далее ожидает нажатия любой клавиши
                while (true)
                {
                    ConsoleKeyInfo kki = Console.ReadKey(false);
                    //if (kki.KeyChar == 0x1B)
                    if (kki.KeyChar != 0x0)
                        break;
                    System.Threading.Thread.Sleep(1000);
                }

                Console.Write("\r\nStop server ");
                try
                {
                    if (m_Thread != null)
                        m_Thread.Abort();
                }
                catch { }
                try
                {
                    m_Listener.Stop();
                    Console.WriteLine("OK");
                }
                catch (Exception ex)
                {
                    Console.Write(" <- ERROR: {0} ", ex.Message);
                }
                Console.WriteLine(" Good bye!");
           
        }

        static HttpListener m_Listener;
        static Thread m_Thread;

        /// <summary>
        /// Обработчик TCP-запросов
        /// </summary>
        static void _processTCP()
        {
            while (!m_Break)
            {
                HttpListenerContext context = m_Listener.GetContext();
                if (context == null) continue;

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    response.ContentType = request.ContentType;
                    response.ContentEncoding = Encoding.UTF8;
                    ProcessStreamHelper.ProcessXml(request.InputStream, response.OutputStream);
                    sw.Stop();
                    Console.WriteLine("Process text by {0} ms  ", sw.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: {0} ", ex.Message);
                    string res = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><error>{0}</error>", ex.Message);
                    byte[] bres = Encoding.UTF8.GetBytes(res);
                    try
                    {
                        response.ContentType = request.ContentType;
                        response.ContentEncoding = Encoding.UTF8;
                        response.OutputStream.Write(bres, 0, bres.Length);
                    }
                    catch { }
                }
                try
                {
                    response.Close();
                }
                catch { }
            }
        }

        static bool m_Break;
        static void _keyPress()
        {
            m_Break = false;
            while (!m_Break)
            {
                ConsoleKeyInfo kki = Console.ReadKey(false);
                if (kki.KeyChar == 0x1B) m_Break = true;
                System.Threading.Thread.Sleep(1000);
            }

        }
    }
}
