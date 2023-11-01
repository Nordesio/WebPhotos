using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using KPO_Cursovaya.StorageInterfaces;
using KPO_Cursovaya.Models;
using KPO_Cursovaya.Implements;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net;
using EO.WebBrowser.DOM;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace WebApp.Controllers
{
    public class VkController : Controller
    {
        private readonly IUserStorage _userStorage;
        private readonly IVkuserStorage _vkuserStorage;
        public VkController(IUserStorage userStorage, IVkuserStorage vkuserStorage)
        {
            _userStorage = userStorage;
            _vkuserStorage = vkuserStorage;
        }

        // GET: VkController
        public ActionResult Index()
        {
            return View(_vkuserStorage.GetListByUser(HomeController.auth_user.Id));
        }

        // GET: VkController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VkController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VkController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VkController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VkController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VkController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VkController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
