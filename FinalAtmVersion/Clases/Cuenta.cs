using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAtmVersion.Clases
{
    public class Cuenta
    {
        public int NumeroCuenta { get; set; }
        public string Pin { get; set; }
        public decimal Saldo { get; set; }
        public List<string> Movimientos { get; set; } = new List<string>();

        public Cuenta(int numero, string pin, decimal saldoInicial = 0)
        {
            NumeroCuenta = numero;
            Pin = pin;
            Saldo = saldoInicial;
        }

        public void AgregarMovimiento(string tipo, decimal monto)
        {
            string fechaHora = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            string movimiento = $"{fechaHora}:{tipo}:{monto}:{Saldo}";

            Movimientos.Add(movimiento);
            if (Movimientos.Count > 5)
            {
                Movimientos = Movimientos.Skip(Movimientos.Count - 5).ToList();
            }


        }

    }
}
