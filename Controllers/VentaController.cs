using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCOREM4DatabaseFirst.DataAccess;
using NETCOREM4DatabaseFirst.Models;
using Newtonsoft.Json;


namespace NETCOREM4DatabaseFirst.Controllers
{
    public class VentaController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.ListadoCliente = await DACliente.listadoAysnc();
            ViewBag.ListadoProducto = await DAProducto.listadoAsync();
            return View();
        }

        public async Task<IActionResult> ListadoOrdenes()
        {
            ViewBag.ListadoOrden = await DAOrden.listadoAsyncSP();
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

            if (listado.Where(x=>x.ProductId== productID).FirstOrDefault() !=null)
            {
                return StatusCode((int)HttpStatusCode.AlreadyReported, Json("DUP"));

            }

            OrderItem detalle = new OrderItem();
            detalle.ProductId = productID;
            detalle.Quantity = quantity;
            detalle.UnitPrice = unitPrice;

            listado.Add(detalle);

            HttpContext.Session.SetString("listaProducto", JsonConvert.SerializeObject(listado));

            return StatusCode((int)HttpStatusCode.AlreadyReported, Json("OK"));
        }




    }
}
