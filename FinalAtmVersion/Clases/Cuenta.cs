using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAtmVersion.Clases
{
    internal class Cuenta
    {
        public int NumeroCuenta { get; set; }
        public string Pin { get; set; }
        public decimal Saldo { get; set; }
        public List<string> Movimientos { get; set; } = new List<string>();
    }
}
