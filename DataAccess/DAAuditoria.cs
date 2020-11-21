using NETCOREM4DatabaseFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.DataAccess
{
    public class DAAuditoria
    {

        public static bool Insertar(Audit audit)
        {
            bool exito = true;
            try
            {
                using (var data = new SalesContext())
                {
                    data.Audits.Add(audit);
                    data.SaveChanges();
                }

            }
            catch (Exception)
            {
                exito = false;
            }

            return exito;
        }

    }
}
