using NETCOREM4DatabaseFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.DataAccess
{
    public class DAUsuario
    {
        public static User Validar(string correo, string clave)
        {
            User user = null;
            using (var data = new SalesContext())
            {
                user = data.Users.Where(x => x.email.Equals(correo) && x.pwd.Equals(clave) && x.state == true).FirstOrDefault();
            }
            return user;
        }



    }
}
