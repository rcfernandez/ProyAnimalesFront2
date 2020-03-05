using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProyAnimalesFront2.Models;
using ProyectoAnimales.Models;

namespace ProyAnimalesFront2.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        private readonly string URI = "https://localhost:44374/";


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Login(AccessViewModel model)
        {

            if (ModelState.IsValid)
            {
                HttpClient http = new HttpClient();

                var data = new { Email = model.Email, Pass = model.Pass };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(URI + "api/access/login/", stringContent);
                var result = response.Content.ReadAsStringAsync().Result;
                Reply oR = JsonConvert.DeserializeObject<Reply>(result);


                if (oR.Result == 1)
                {
                    var session = HttpContext.Session;
                    var tokenHeader = response.Headers.GetValues("token").First();

                    session.SetString("sessionToken", tokenHeader);

                    return RedirectToAction("Index");
                }

            }



            // si ocurre un error
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
