using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCOREM4DatabaseFirst.DataAccess;
using NETCOREM4DatabaseFirst.Models;
using Newtonsoft.Json;

namespace NETCOREM4DatabaseFirst.Controllers
{
    public class SeguridadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Clave)
        {
            User user = DAUsuario.Validar(Email, Clave);

            if (user == null)
                return RedirectToAction("Index");

            HttpContext.Session.SetString("objUsuario", JsonConvert.SerializeObject(user));

            return RedirectToAction("Index","Venta");

        }
    }
}
