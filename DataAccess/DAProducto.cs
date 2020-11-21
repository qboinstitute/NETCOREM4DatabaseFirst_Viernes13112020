using Microsoft.EntityFrameworkCore;
using NETCOREM4DatabaseFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.DataAccess
{
    public class DAProducto
    {
        public static List<Product> listado()
        {
            using (var data = new SalesContext())
            {
                return data.Products.ToList();
            }

        }

        public static async Task<List<Product>> listadoAsync()
        {
            using (var data = new SalesContext())
            {
                return await data.Products.OrderBy(x=>x.ProductName).ToListAsync();
            }

        }



    }
}
