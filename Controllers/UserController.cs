using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmacyMedicineSupply.Models;

namespace PharmacyMedicineSupply.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User credentials)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(credentials),Encoding.UTF8,"application/json");
                var response = await httpClient.PostAsync("https://localhost:44300/User/Login", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JWT token = JsonConvert.DeserializeObject<JWT>(result);
                    HttpContext.Session.SetString("token", token.Token);
                    HttpContext.Session.SetString("userName", credentials.UserName);
                    ViewBag.UserName = credentials.UserName;
                    return View("Index");
                }
                else
                {
                    ViewBag.Info = "Invalid username/password";
                    return View();
                }
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("userName");
            return View();
        }
    }
}