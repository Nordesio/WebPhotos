using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbData;
using DbData.Implements;
using DbData.StorageInterfaces;
using DbData.Models;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace RecBot
{
    class Program
    {

        static private IRequestStorage _requestStorage;
        public Program(IRequestStorage requestStorage)
        {
            _requestStorage = requestStorage;
        }
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddScoped<IRequestStorage, RequestStorage>();
            // Создаем поставщик сервисов
            var serviceProvider = services.BuildServiceProvider();

            // Получаем экземпляр IRequestStorage из поставщика сервисов
            var requestStorage = serviceProvider.GetRequiredService<IRequestStorage>();
            var program = new Program(requestStorage);


            Init();

            //while (true)
            //{
                try
                {
                    
                    ProcessNewPosts(requestStorage);
                    Thread.Sleep(1000 * 60 * 5);
                }
                catch (Exception ex)
                {
                    
                }
           // }
            // Создаем экземпляр Program и передаем IRequestStorage в конструктор

        }

        private static void Init()
        {
            DirectoryInfo dir = new DirectoryInfo("C:\\Users\\Public\\Downloads\\Recognizer");
            if (!dir.Exists)
                dir.Create();

            DirectoryInfo dirTemp = new DirectoryInfo("C:\\Users\\Public\\Downloads\\Recognizer\\tmp");
            if (!dirTemp.Exists)
                dirTemp.Create();

           
        }

        private static void ProcessNewPosts(IRequestStorage requestStorage)
        {
            
            // тест метода, можно задать свой список
            string firstId = "";
            var list = _requestStorage.GetFullList();
            foreach (var l in list)
            {

                Task.Run(() =>
                {
                    string text = "";

                        try
                        {
                        string fullLink = l.AuthorLink;

                            RecognizeProcess process = new RecognizeProcess(fullLink);
                            text += '\n' + process.RecognizeText();
                        Console.WriteLine("Получили текст с изображения с id " + l.Id + " text = " + text + "\n");
                            
                        }
                        catch (Exception ex)
                        {
                        Console.WriteLine("Ошибка\n");
                        }
                    

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            l.Text = text; 
                            
                            _requestStorage.Update(l);
                            
                            Console.WriteLine("Записали текст \n");
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка\n");
                        
                    }
                });

            }
        }
    }
}
