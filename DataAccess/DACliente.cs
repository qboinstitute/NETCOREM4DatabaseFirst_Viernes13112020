using Microsoft.EntityFrameworkCore;
using NETCOREM4DatabaseFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.DataAccess
{
    public class DACliente
    {
        public static List<Customer> listado()
        {
            using (var data = new SalesContext())
            {
                return data.Customers.ToList();
            }
        
        }

        public static async Task<List<Customer>> listadoAysnc()
        {
            using (var data = new SalesContext())
            {
                return await data.Customers.ToListAsync();
            }

        }


    }
}
