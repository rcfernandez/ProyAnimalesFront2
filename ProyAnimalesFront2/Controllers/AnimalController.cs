using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoAnimales.Models;
using ProyectoAnimales.Models.EF;

namespace ProyAnimalesFront2.Controllers
{
    public class AnimalController : Controller
    {

        private readonly string URI = "https://localhost:44374/";

        // GET: Animal
        public async Task<IActionResult> Index()
        {

            var http = new HttpClient();
            var sesionToken = HttpContext.Session.GetString("sessionToken");
            
            var content = new StreamContent(Stream.Null);
            content.Headers.Add("Content-Type", "application/json");
            content.Headers.Add("token", sesionToken);

            var requestMsg = new HttpRequestMessage(HttpMethod.Get, URI + "api/animals/get/") { Content = content };
            var response = await http.SendAsync(requestMsg);
            var result = response.Content.ReadAsStringAsync().Result;

            Reply oR = JsonConvert.DeserializeObject<Reply>(result);

            TempData["msg"] = oR.Message;

            return View(oR);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Animal/Create
        [HttpPost]
        public async Task<IActionResult> Create(AnimalitoViewModel model)
        {

            if (ModelState.IsValid)
            {
                TempData["msg"] = null;

                HttpClient http = new HttpClient();

                var data = new { Nombre = model.Nombre, Patas = model.Patas, IdEstado = model.IdEstado };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Add("token", HttpContext.Session.GetString("sessionToken"));

                var requestMsg = new HttpRequestMessage(HttpMethod.Post, URI + "Api/Animals/Add") { Content = content };
                var response = await http.SendAsync(requestMsg);
                var result = response.Content.ReadAsStringAsync().Result;
                Reply oR = JsonConvert.DeserializeObject<Reply>(result);

                if (oR.Result == 1)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msg"] = oR.Message;

                    return View();
                }

            }

            // si ocurre un error
            //TempData["msg"] = "El modelo en invalido";
            return View();

        }



        // GET: Animal/Buscar
        public async Task<JsonResult> Exist(string nombre)
            {

            var http = new HttpClient();
            var sesionToken = HttpContext.Session.GetString("sessionToken");

            var content = new StreamContent(Stream.Null);
            content.Headers.Add("Content-Type", "application/json");
            content.Headers.Add("token", sesionToken);

            var requestMsg = new HttpRequestMessage(HttpMethod.Get, URI + "api/animals/exist/" + nombre) { Content = content };
            var response = await http.SendAsync(requestMsg);
            var result = response.Content.ReadAsStringAsync().Result;

            if (result == "true")
            {
                return Json("1");
            }

            return Json("0");

        }




    }
}