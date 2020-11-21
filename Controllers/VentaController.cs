using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCOREM4DatabaseFirst.DataAccess;
using NETCOREM4DatabaseFirst.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NETCOREM4DatabaseFirst.Controllers
{
    public class VentaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.ListadoCliente = await DACliente.listadoAysnc();
            ViewBag.ListadoProducto = await DAProducto.listadoAsync();
            var usuario = HttpContext.Session.GetString("objUsuario");
            User user = JsonConvert.DeserializeObject<User>(usuario);
            ViewData["NombreUsuario"] = user.fullname;

            string tcEuro;

            using (var httpClient = new HttpClient())
            {
                using (var respuesta = await httpClient.GetAsync("https://api.exchangeratesapi.io/latest?base=USD"))
                {
                    string apiRespuesta = await respuesta.Content.ReadAsStringAsync();

                    tcEuro = (string)JObject.Parse(apiRespuesta)["rates"]["EUR"];


                }

            }

            ViewBag.TipoCambioEUR = tcEuro;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarOrden(int customerID,
            DateTime orderDate, string orderNumber)
        {
            Order cabecera = new Order();
            cabecera.CustomerId = customerID;
            cabecera.OrderDate = orderDate;
            cabecera.OrderNumber = orderNumber;

            List<OrderItem> detalle = new List<OrderItem>();
            var productos = HttpContext.Session.GetString("listaProducto");
            detalle = JsonConvert.DeserializeObject<List<OrderItem>>(productos);
            if (detalle.Count() == 0)
                return Json("NP");

            int newOrderID = await DAOrden.Insertar(cabecera, detalle);
            //HttpContext.Session.SetString("listaProducto", "");

            var usuario = HttpContext.Session.GetString("objUsuario");
            User user = JsonConvert.DeserializeObject<User>(usuario);

            Audit audit = new Audit();
            audit.idorden = newOrderID;
            audit.fecha = DateTime.Now;
            audit.idusuario = user.id;
            bool exito = DAAuditoria.Insertar(audit);

            return Json(newOrderID);
        }

        public async Task<IActionResult> ListadoOrdenes()
        {
            ViewBag.ListadoOrden = await DAOrden.listadoAsyncSP();
            ViewBag.ListadoCliente = await DACliente.listadoAysnc();
            return PartialView();
        }

        public IActionResult QuitarProductoOrden(int productID)
        {
            List<OrderItem> listado;
            var productos = HttpContext.Session.GetString("listaProducto");
            if (productos == null)
                listado = new List<OrderItem>();
            else
                listado = JsonConvert.DeserializeObject<List<OrderItem>>(productos);

            OrderItem item = listado.Where(x => x.ProductId == productID).FirstOrDefault();
            listado.Remove(item);
            HttpContext.Session.SetString("listaProducto", JsonConvert.SerializeObject(listado));
            return Json("OK");
        }


        public async Task<IActionResult> ListadoProducto()
        {
            List<OrderItem> listado;
            var productos = HttpContext.Session.GetString("listaProducto");
            if (productos == null)
                listado = new List<OrderItem>();
            else
                listado = JsonConvert.DeserializeObject<List<OrderItem>>(productos);

            ViewBag.ProductosAgregados = listado;
            ViewBag.ListadoProducto = await DAProducto.listadoAsync();

            return PartialView();
        }

        [HttpPost]
        public IActionResult AgregarProducto(int productID, decimal unitPrice, int quantity)
        {
            List<OrderItem> listado;
            var productos = HttpContext.Session.GetString("listaProducto");
            if (productos == null)
            {
                listado = new List<OrderItem>();
            }
            else
            {
                listado = JsonConvert.DeserializeObject<List<OrderItem>>(productos);
            }

            if (listado.Where(x => x.ProductId == productID).Count() > 0)
            {
                return Json("DUP");

            }

            OrderItem detalle = new OrderItem();
            detalle.ProductId = productID;
            detalle.Quantity = quantity;
            detalle.UnitPrice = unitPrice;

            listado.Add(detalle);

            HttpContext.Session.SetString("listaProducto", JsonConvert.SerializeObject(listado));

            return Json("OK");
        }




    }
}
