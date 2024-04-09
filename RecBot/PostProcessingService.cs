using DbData.Implements;
using DbData.Models;
using DbData.StorageInterfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Server;
using System.Diagnostics;

namespace RecBot
{
    public interface IPostProcessingService
    {
        void ProcessNewPosts(List<Request> list);
    }

    public class PostProcessingService : IPostProcessingService
    {
        private readonly IRequestStorage _requestStorage;

        private readonly IEntitySearchService _entitySearchService; // Сервис для поиска сущностей

        public PostProcessingService(IRequestStorage requestStorage, IEntitySearchService entitySearchService)
        {
            _requestStorage = requestStorage;
            _entitySearchService = entitySearchService;
        }

        public void ProcessNewPosts(List<Request> list)
        {
            
            Init();

            //while (true)
            //{
            try
            {
                Task.Run(async () => { await ProcessPostsAsync(list); });
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
        private async Task ProcessPostsAsync(List<Request> list)
        {
            Stopwatch stopwatch = new Stopwatch();
            int firstId = -1;

            foreach (var l in list)
            {
                if (firstId == -1)
                {
                    firstId = l.Id;
                    stopwatch.Start();
                }

                try
                {
                    string fullLink = l.ImageLink;
                    RecognizeProcess process = new RecognizeProcess(fullLink);
                    string text = await Task.Run(() => process.RecognizeText());
                    text = "Правительство России задумало вернуть налог на движимое имущество, который отменили с 1 января 2019 года, еще при предыдущем составе кабмина. Об этом заявил замминистра финансов Алексей Лавров, пишут «Ведомости». Спикер Совета Федерации Валентина Матвиенко напомнила, что ранее предупреждали о негативных последствиях налогового послабления: «Освободить от налога стоило только новое оборудование, приобретаемое компаниями, но нас не послушали, все освободили». В 2019 году Минфин оценил выпадающие доходы в 183 миллиарда рублей. Компенсировать их предполагалось за счет роста цен на крепкий алкоголь, но добиться этого не удалось.";

                    Console.WriteLine($"Получили текст с изображения с id {l.Id}: {text}");

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        l.Text = text;
                        _requestStorage.Update(l);
                        _entitySearchService.SearchEntities(l);
                        Console.WriteLine("Записали текст");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            // Засекаем время выполнения
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;
            Console.WriteLine($"Время выполнения для первого запроса с id {firstId}: {elapsedTime}");
        }

    }

}
