using DbData.Implements;
using DbData.Models;
using DbData.StorageInterfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RecBot
{
    public interface IPostProcessingService
    {
        void ProcessNewPosts(List<Request> list);
    }

    public class PostProcessingService : IPostProcessingService
    {
        private readonly IRequestStorage _requestStorage;

        public PostProcessingService(IRequestStorage requestStorage)
        {
            _requestStorage = requestStorage;
        }

        public void ProcessNewPosts(List<Request> list)
        {
            
            Init();

            //while (true)
            //{
            try
            {
                ProcessPosts(list);
                //Thread.Sleep(1000 * 60 * 5);
            }
            catch (Exception ex)
            {

            }
            // }


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
        private void ProcessPosts(List<Request> list)
        {
            
            string firstId = "";
            //var list = _requestStorage.GetFullList();
            foreach (var l in list)
            {

                Task.Run(() =>
                {
                    string text = "";

                    try
                    {
                        string fullLink = l.ImageLink;

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
                            //вызывать поиск сущностей
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
