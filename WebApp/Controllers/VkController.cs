﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using KPO_Cursovaya.StorageInterfaces;
using KPO_Cursovaya.Models;
using KPO_Cursovaya.Implements;
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
using X.PagedList.Mvc;
namespace WebApp.Controllers
{
    public class VkController : Controller
    {
        private readonly IUserStorage _userStorage;
        private readonly IVkuserStorage _vkuserStorage;
        private readonly IRequestStorage _requestStorage;
        public VkController(IUserStorage userStorage, IVkuserStorage vkuserStorage, IRequestStorage requestStorage)
        {
            _userStorage = userStorage;
            _vkuserStorage = vkuserStorage;
            _requestStorage = requestStorage;
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

        public async Task DownloadPicsFromVkUser(Vkuser vkuser)
        {
            
            List<Request> Requests = new List<Request>();
            string first_name = "";
            string last_name = "";
            string https = vkuser.Url;
            string id_stroka = https.Remove(0, 15);
            string token = "vk1.a.j7qTt6XmHpN0toO7XpLuNC4dJ2_xk1PYZNMcVlW2_Nse0FnFfTjpkZS7fKhTnR_tTa90qYykgeEKm9HtOfEY4wg__p-Iy63ailP2r1OXZv-tTsHxB44crQPOQnzHodfkj5lRXl8QC20lpyESNNSD676yuVCab18W9-EsSenOcXH4-gf_j5vUCeR6SDSyh99xQSeFwR-6Uue9E01wTihbUw";
            int offset = 0;
            string getUserId = string.Format("https://api.vk.com/method/users.get?access_token={0}&user_ids={1}&v=5.131", token, id_stroka);
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


            bool toomuch = false;
            string text = "";
            List<string> urls = new List<string>();
            List<DateTime> dates = new List<DateTime>();
            string texts = "";
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
                var texts_from_url = JsonObject.SelectTokens("response.items[" + i + "].text")
              .Select(v => (string)v)
              .ToList();
                foreach (var ur in texts_from_url)
                {
                    texts = ur;
                }
                Request request = new Request();
                request.VkuserId = vkuser.Id;
                request.Date = DateOnly.FromDateTime((DateTime)d);
                request.Text = texts;
                request.ImageByte = DownloadImageToByteArray(u);
                request.Url = ByteToString(request.ImageByte);
                Console.WriteLine("Скачано: " + u);
                request.Author = last_name + " " + first_name;
                request.AuthorId = owner_id;
                request.AuthorLink = "https://vk.com/" + owner_id;
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
                    var texts_from_url = JsonObject.SelectTokens("response.items[" + i + "].text")
                  .Select(v => (string)v)
                  .ToList();
                    foreach (var ur in texts_from_url)
                    {
                        texts = ur;
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
                    
                    request.Text = texts;
                    request.ImageByte = DownloadImageToByteArray(u);
                    request.Url = ByteToString(request.ImageByte);
                    request.Author = first_name + " " + last_name;
                    request.AuthorId = "id" + owner_id;
                    request.AuthorLink = "https://vk.com/id" + owner_id;
                    Requests.Add(request);
                }

            }
           
            Console.WriteLine("Скачано {0} фотографий у id{1}", Requests.Count, owner_id);
            _requestStorage.AddFullList(Requests);
            vkuser.Status = "completed";
            _vkuserStorage.Update(vkuser);
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
        [HttpGet]
        public ActionResult VkResult(int vk_id, int? id = 1)
        {
            var images = _requestStorage.GetByVkId(vk_id);
            List<string> imagesList = new List<string>();
            foreach(var i in images)
            {
                if(!i.Url.Equals("null"))
                imagesList.Add(i.Url);
            }
            int pageNumber = (id ?? 1);
            ViewBag.Images = imagesList;
            return View("Images", imagesList.ToPagedList(pageNumber, 30));
        }
       [HttpGet]
       public IActionResult AddVkUser()
        {
            return View();
        }
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
            
            await DownloadPicsFromVkUser(vkuser);
            //new Thread(() => { DownloadPicsFromVkUser; })
            return Redirect(nameof(Index));
        }
        [HttpGet]
        public ActionResult DeleteVkUser(int vk_id)
        {
            Vkuser vkuser = new Vkuser();
            vkuser = _vkuserStorage.GetById(vk_id);
            return View(vkuser);
        }
        public ActionResult DeleteConfirmed(int vk_id)
        {
            _vkuserStorage.Delete(vk_id);
            return Redirect(nameof(Index));
        }
    //    VkApi vkApi = new VkApi();
    //    vkApi.Authorize(new VkNet.Model.ApiAuthParams
    //        {
    //            ApplicationId = 7087011,
    //            Login = "email",
    //            Password = "pass",
    //            Settings = VkNet.Enums.Filters.Settings.All
    //});
    //        string token = vkApi.Token;
}
}
