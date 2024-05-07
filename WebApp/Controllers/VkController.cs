using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using DbData.StorageInterfaces;
using DbData.Models;
using DbData.Implements;
using System.Globalization;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using X.PagedList;
using RecBot;
using System.Runtime.InteropServices;




namespace WebApp.Controllers
{
    public class VkController : Controller
    {
        private readonly IUserStorage _userStorage;
        private readonly IVkuserStorage _vkuserStorage;
        private readonly IRequestStorage _requestStorage;
        private readonly IEntityStorage _entityStorage;
        private readonly IPostProcessingService _postProcessingService;
        public VkController(IUserStorage userStorage, IVkuserStorage vkuserStorage, IRequestStorage requestStorage, IPostProcessingService postProcessingService, IEntityStorage entityStorage)
        {
            _userStorage = userStorage;
            _vkuserStorage = vkuserStorage;
            _requestStorage = requestStorage;
            _postProcessingService = postProcessingService;
            _entityStorage = entityStorage; 
        }

        public byte[]? DownloadImageToByteArray(string imageUrl)
        {
            byte[]? imageBytes = null;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    imageBytes = webClient.DownloadData(imageUrl);
                }catch(Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }
            }
            return imageBytes;
        }
        public string ByteToString(byte[]? imageBytes)
        {
            if(imageBytes == null)
            {
                return "null";
            }
            string imageBase64 = Convert.ToBase64String(imageBytes);
            string imageSrc = $"data:image/jpeg;base64,{imageBase64}";
            return imageSrc;
        }
        public async Task<List<Request>> Parse(string last_name, string first_name, string token, string owner_id, int offset, Vkuser vkuser)
        {
            List<Request> Requests = new List<Request>();
            bool toomuch = false;
            string text = "";
            List<string> urls = new List<string>();
            List<DateTime> dates = new List<DateTime>();
            string url = string.Format("https://api.vk.com/method/photos.getAll?access_token={0}&owner_id={1}&extended=1&offset={2}&count=200&photo_sizes=1&no_service_albums=0&need_hidden=0&skip_hidden=1&v=5.131", token, owner_id, offset);
            var parsed = JsonConvert.DeserializeObject(new WebClient().DownloadString(url));
            text = parsed.ToString();
            var JsonObject = JToken.Parse(text);
            int num_pics = 0;
            var num_of_pics = JsonObject.SelectTokens("response.count")
              .Select(v => (string)v)
              .ToList();
            foreach (var ur in num_of_pics)
            {
                num_pics = Int32.Parse(ur);
            }
            if (num_pics > 200)
                toomuch = true;
            int number_of_parsing = num_pics / 200;
            int num_pars = number_of_parsing;
            int max_pics = 0;
            int num = 0;
            if (toomuch == true)
                max_pics = 200;
            else
                max_pics = num_pics;
            for (int i = 0; i < max_pics; i++)
            {
                string u = "";
                DateTime? d = null;
                var heights = JsonObject.SelectTokens("response.items[" + i + "].sizes[*].height")
               .Select(v => (string)v)
               .ToList();
                int schetchik = 0;
                int temp = 0;
                foreach (var ur in heights)
                {
                    if (Int32.Parse(ur) >= temp)
                    {
                        num = schetchik;
                        temp = Int32.Parse(ur);
                    }
                    schetchik++;
                }
                var pic_urls = JsonObject.SelectTokens("response.items[" + i + "].sizes[" + num + "].url")
               .Select(v => (string)v)
               .ToList();
                foreach (var ur in pic_urls)
                {
                    u = ur;
                    urls.Add(ur);
                }
                var dates_from_urls = JsonObject.SelectTokens("response.items[" + i + "].date")
               .Select(v => (string)v)
               .ToList();
                foreach (var ur in dates_from_urls)
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(Double.Parse(ur)).ToLocalTime();

                    d = dateTime;
                    dates.Add(dateTime);
                }

                Request request = new Request();
                request.VkuserId = vkuser.Id;
                request.Date = DateOnly.FromDateTime((DateTime)d);
                request.ImageByte = DownloadImageToByteArray(u);
                request.Url = ByteToString(request.ImageByte);
                request.ImageLink = u;
                Console.WriteLine("Скачано: " + u);
                request.Author = last_name + " " + first_name;
                request.AuthorId = owner_id;
                // временно записываю ссылку на фото в эту переменную
                request.AuthorLink = "https://vk.com/id" + owner_id;
                Requests.Add(request);
            }


            while (number_of_parsing > 0)
            {
                if (number_of_parsing == 1)
                {
                    max_pics = num_pics - (200 * num_pars);
                    //Console.WriteLine(max_pics);
                    //Console.WriteLine(num_pics);

                }
                number_of_parsing--;
                offset += 200;
                url = string.Format("https://api.vk.com/method/photos.getAll?access_token={0}&owner_id={1}&extended=1&offset={2}&count=200&photo_sizes=1&no_service_albums=0&need_hidden=0&skip_hidden=1&v=5.131", token, owner_id, offset);
                parsed = JsonConvert.DeserializeObject(new WebClient().DownloadString(url));
                text = parsed.ToString();

                JsonObject = JToken.Parse(text);
                num = 0;
                for (int i = 0; i < max_pics; i++)
                {
                    string u = "";
                    DateTime? d = null;
                    var heights = JsonObject.SelectTokens("response.items[" + i + "].sizes[*].height")
                   .Select(v => (string)v)
                   .ToList();
                    int schetchik = 0;
                    int temp = 0;
                    foreach (var ur in heights)
                    {
                        if (Int32.Parse(ur) >= temp)
                        {
                            num = schetchik;
                            temp = Int32.Parse(ur);
                        }
                        schetchik++;
                    }
                    var pic_urls = JsonObject.SelectTokens("response.items[" + i + "].sizes[" + num + "].url")
                   .Select(v => (string)v)
                   .ToList();
                    foreach (var ur in pic_urls)
                    {
                        u = ur;
                        urls.Add(ur);
                    }
                    var dates_from_urls = JsonObject.SelectTokens("response.items[" + i + "].date")
                   .Select(v => (string)v)
                   .ToList();
                    foreach (var ur in dates_from_urls)
                    {
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddSeconds(Double.Parse(ur)).ToLocalTime();

                        d = dateTime;
                        dates.Add(dateTime);
                    }

                    Request request = new Request();
                    request.VkuserId = vkuser.Id;
                    if (d.HasValue)
                    {
                        request.Date = DateOnly.FromDateTime((DateTime)d);
                    }
                    else
                    {
                        request.Date = null;
                    }

                    request.ImageByte = DownloadImageToByteArray(u);
                    request.Url = ByteToString(request.ImageByte);
                    request.ImageLink = u;
                    request.Author = first_name + " " + last_name;
                    request.AuthorId = "id" + owner_id;
                    // временно записываю ссылку на фото в эту переменную
                    request.AuthorLink = "https://vk.com/id" + owner_id;
                    Requests.Add(request);
                }

            }
            Console.WriteLine("Скачано {0} фотографий у id{1}", Requests.Count, owner_id);
            return Requests;
        }
        public async Task DownloadPicsFromVkUser(Vkuser vkuser)
        {
            
            
            string first_name = "";
            string last_name = "";
            string https = vkuser.Url;
            string id_stroka = https.Remove(0, 15);
            int offset = 0;
            string getUserId = string.Format("https://api.vk.com/method/users.get?access_token={0}&user_ids={1}&v=5.131", HomeController.token, id_stroka);
            string owner_id = "";
            var parsed123 = JsonConvert.DeserializeObject(new WebClient().DownloadString(getUserId));
            var JsonObject2 = JToken.Parse(parsed123.ToString());
            Console.WriteLine(parsed123.ToString());

            var ids = JsonObject2.SelectTokens("response..id")
                 .Select(v => (string)v)
                 .ToList();
            foreach (var ur in ids)
            {
                owner_id = ur;
                Console.WriteLine(owner_id);
            }
            var names = JsonObject2.SelectTokens("response..first_name")
                .Select(v => (string)v)
                .ToList();
            foreach (var ur in names)
            {
                first_name = ur;

                Console.WriteLine(first_name);
            }
            var families = JsonObject2.SelectTokens("response..last_name")
                .Select(v => (string)v)
                .ToList();
            foreach (var ur in families)
            {
                last_name = ur;

                Console.WriteLine(last_name);
            }




            await AddToDb(vkuser ,await Parse(last_name, first_name, HomeController.token, owner_id, offset, vkuser));
            
        }

        public async Task AddToDb(Vkuser vkuser, List<Request> requests)
        {
            _requestStorage.AddFullList(requests);
            vkuser.Status = "completed";
            _vkuserStorage.Update(vkuser);
            Thread.Sleep(5000);
            _postProcessingService.ProcessNewPosts(requests);
        }

        private bool IsUserInRules(int vk_id)
        {
            bool user = false;
            var vkuser_test = _vkuserStorage.GetById(vk_id);
            if (HomeController.auth_user.Id != vkuser_test.UserId && HomeController.auth_user.Role != "admin")
            {

            }
            else { user = true; }
            return user;
        }

        // GET: VkController
        public ActionResult Index()
        {
            if (HttpContext.User.IsInRole("user"))
            {
                //Request req = new Request();
                //req.VkuserId = 0;
                //req.ImageByte = DownloadImageToByteArray("https://sportishka.com/uploads/posts/2022-04/1650672260_2-sportishka-com-p-ssha-san-frantsisko-krasivo-foto-2.jpg");
                //_requestStorage.Insert(req);
                //DownloadPicsFromVkUser(1);
                return View(_vkuserStorage.GetListByUser(HomeController.auth_user.Id));
            }
            else
            {
                return View(_vkuserStorage.GetFullList());
            }
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public ActionResult VkResult(int vk_id, int? id = 1)
        {
            if (!IsUserInRules(vk_id))
            {
                return RedirectToAction(nameof(Index));
            }
            var images = _requestStorage.GetByVkId(vk_id);
            List<Request> imagesList = new List<Request>();

            var filteredImages = images.Where(i => !i.Url.Equals("null")).ToList();

            int pageNumber = (id ?? 1);

            ViewBag.Images = filteredImages;
            ViewBag.Id = vk_id;
            ViewBag.Name = filteredImages.FirstOrDefault()?.Author; // Получаем имя автора первого изображения, если оно есть
            return View(filteredImages.ToPagedList(pageNumber, 30));
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
       public IActionResult AddVkUser()
        {
            return View();
        }
        [Authorize(Roles = "user, admin")]
        [HttpPost]
        public async Task<IActionResult> AddVkUser(string Name, string Url)
        {
            
            Vkuser vkuser = new Vkuser();
            vkuser.Name = Name;
            vkuser.Url = Url;
            vkuser.UserId = HomeController.auth_user.Id;
            DateTime date = DateTime.Now;
            vkuser.Date = DateOnly.FromDateTime((DateTime)date);
            vkuser.Status = "in_progress";
            try
            {
                _vkuserStorage.Insert(vkuser);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Task.Run(async () => { await DownloadPicsFromVkUser(vkuser); });

            return Redirect(nameof(Index));
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public ActionResult DeleteVkUser(int vk_id)
        {
            if (!IsUserInRules(vk_id))
            {
                return RedirectToAction(nameof(Index));
            }
            Vkuser vkuser = new Vkuser();
            vkuser = _vkuserStorage.GetById(vk_id);
            return View(vkuser);
        }
        public ActionResult DeleteConfirmed(int vk_id)
        {
            if (!IsUserInRules(vk_id))
            {
                return RedirectToAction(nameof(Index));
            }
            _vkuserStorage.Delete(vk_id);
            return Redirect(nameof(Index));
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public ActionResult EditVkUser(int vk_id)
        {
            if (!IsUserInRules(vk_id))
            {
                return RedirectToAction(nameof(Index));
            }
            Vkuser vkuser = new Vkuser();
            vkuser = _vkuserStorage.GetById(vk_id);
            return View(vkuser);
        }
        [Authorize(Roles = "user, admin")]
        [HttpPost]
        public async Task<IActionResult> EditVkUser(Vkuser vkuser)
        {
            var vkuserold = _vkuserStorage.GetById(vkuser.Id);
            if(vkuserold.Url == vkuser.Url)
            {
                _vkuserStorage.Update(vkuser);
            }
            else
            {
                _requestStorage.DeleteByVkUser(vkuser);
                vkuser.Status = "in_progress";
                try
                {
                    _vkuserStorage.Update(vkuser);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Task.Run(async () => { await DownloadPicsFromVkUser(vkuser); });
            }
            return Redirect(nameof(Index));
        }
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public ActionResult VkPhoto(int vk_id, int imageId)
        {
            // Загрузка данных о фотографии по imageId
            var image = _requestStorage.GetById(imageId);
            try
            {
                var entities = _entityStorage.GetByRequestId(imageId);
                ViewBag.entities = entities;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка - " + ex);
            }

            
            if (image == null)
            {
                return Redirect(nameof(Index));
            }

            return View(image);
        }

        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public ActionResult VkStatistic(int vk_id)
        {
            var images = _requestStorage.GetByVkId(vk_id);
            Dictionary<string, int> entityStatistics = new Dictionary<string, int>();
            string mostCommonEntity = null;
            string mostCommonWords = null;
            int maxEntityCount = 0;

            foreach (var image in images)
            {
                Dictionary<string, string> entities = ExtractEntitiesFromPhoto(image.Id);
                if(entities != null)
                {

                foreach (var entity in entities)
                {
                    if (!entityStatistics.ContainsKey(entity.Key))
                    {
                        entityStatistics[entity.Key] = 0;
                    }
                    entityStatistics[entity.Key]++;

                    if (entityStatistics[entity.Key] > maxEntityCount)
                    {
                        maxEntityCount = entityStatistics[entity.Key];
                        mostCommonEntity = entity.Key;
                        mostCommonWords = entity.Value;
                    }
                }
                
                }
            }
            ViewBag.mostCommonEntity = mostCommonEntity;
            ViewBag.mostCommonWords = mostCommonWords;
            return View(entityStatistics);
        }
        public Dictionary<string, string> ExtractEntitiesFromPhoto(int image_id)
        {
            Dictionary<string, string> entities = new Dictionary<string, string>();
            var entity = _entityStorage.GetByRequestId(image_id);
            if( entity != null )
            {

            if (entity.Geo != null)
            {
                entities.Add("Геолокация", entity.Geo);
            }
            if (entity.Date != null)
            {
                entities.Add("Дата", entity.Date);
            }
            if (entity.Organization != null)
            {
                entities.Add("Организация", entity.Organization);
            }
            if (entity.Namedentity != null)
            {
                entities.Add("Наименование", entity.Namedentity);
            }
            if (entity.Person != null)
            {
                entities.Add("Личность", entity.Person);
            }
            if (entity.Street != null)
            {
                entities.Add("Улица", entity.Street);
            }
            if (entity.Money != null)
            {
                entities.Add("Валюта", entity.Money);
            }
            if (entity.Address != null)
            {
                entities.Add("Адрес", entity.Address);
            }

            }
            return entities;
        }

    }
}
