using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCOREM4DatabaseFirst.Models
{
    public class Audit
    {
        public int id { get; set; }

        public int idusuario { get; set; }

        public int idorden { get; set; }

        public DateTime fecha { get; set; }
    }
}
