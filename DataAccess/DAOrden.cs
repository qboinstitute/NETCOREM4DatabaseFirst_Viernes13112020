using Microsoft.EntityFrameworkCore;
using NETCOREM4DatabaseFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.DataAccess
{
    public class DAOrden
    {
        public static async Task<List<Order>> listadoAsync()
        {
            using (var data = new SalesContext())
            {
                return await data.Orders.ToListAsync();
            }

        }

        public static async Task<List<Order>> listadoAsyncSP()
        {
            using (var data = new SalesContext())
            {
                List<Order> listado = await data.Orders.FromSqlRaw("EXECUTE dbo.SP_SEL_ORDEN ").ToListAsync();
                return listado;
            }

        }


        public static async Task<int> Insertar(Order cabecera, List<OrderItem> detalle)
        {
            //bool exito = true;
            int newID = -1;
            try
            {

                using (var data = new SalesContext())
                {
                    await data.Orders.AddAsync(cabecera);//Id no existe
                    await data.SaveChangesAsync();

                    newID = cabecera.Id;

                    decimal totalAmount = 0;
                    foreach (var item in detalle)
                    {
                        totalAmount = totalAmount + (item.UnitPrice * item.Quantity);
                        item.OrderId = newID;
                    }

                    await data.OrderItems.AddRangeAsync(detalle);
                    cabecera.TotalAmount = totalAmount;
                    await data.SaveChangesAsync();
                }

            }
            catch (Exception)
            {
                newID = -1;
                throw;
            }


            return newID;
        }






    }
}
