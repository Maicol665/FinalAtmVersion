using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAtmVersion.Clases
{
    internal class CajeroAutomatico
    {
        private const string ArchivoCuentas = "cuentas.txt"; 
        private const string ArchivoMovimientos = "movimientos_{0}.txt";
        private Cuenta cuentaActual;

        public void Inicializar()
        {
            if (!File.Exists(ArchivoCuentas))
            {
                using (StreamWriter writer = new StreamWriter(ArchivoCuentas))
                {
                    writer.WriteLine("12345:0000:0");
                }
                Console.WriteLine("Archivo de cuentas creado con cuenta de ejemplo.");
            }
        }

        public bool IniciarSesion()
         {
            Console.Write("Ingrese número de cuenta: ");
            if (!int.TryParse(Console.ReadLine(), out int numeroCuenta))
            {
                Console.WriteLine("Número de cuenta inválido.");
                return false;
            }

            string pinIngresado;
            int intentos = 0;
            const int maxIntentos = 3;

            while (intentos < maxIntentos)
            {
                Console.Write("Ingrese PIN (4 dígitos): ");
                pinIngresado = Console.ReadLine();

                if (pinIngresado.Length != 4 || !ValidarLogin(numeroCuenta, pinIngresado))
                {
                    intentos++;
                    Console.WriteLine($"PIN incorrecto. Intentos restantes: {maxIntentos - intentos}");
                    if (intentos == maxIntentos)
                    {
                        Console.WriteLine("Sesión bloqueada. Intente más tarde.");
                        return false;
                    }
                }
                else
                {
                    CargarCuenta(numeroCuenta);
                    Console.WriteLine("Login exitoso. Bienvenido.");
                    return true;
                }
            }
            return false;
         }


        private bool ValidarLogin(int numero, string pin)
        {
            if (!File.Exists(ArchivoCuentas)) return false;

            string[] lineas = File.ReadAllLines(ArchivoCuentas);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split(':');
                if (partes.Length == 3 && int.Parse(partes[0]) == numero && partes[1] == pin)
                {
                    return true;
                }
            }
            return false;
        }

        private void CargarCuenta(int numero)
        {
            string[] lineas = File.ReadAllLines(ArchivoCuentas);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split(':');
                if (partes.Length == 3 && int.Parse(partes[0]) == numero)
                {
                    cuentaActual = new Cuenta(numero, partes[1], decimal.Parse(partes[2]));
                    CargarMovimientos();
                    return;
                }
            }
            throw new InvalidOperationException("Cuenta no encontrada.");
        }

        private void CargarMovimientos()
        {
            string archivoMov = string.Format(ArchivoMovimientos, cuentaActual.NumeroCuenta);
            if (File.Exists(archivoMov))
            {
                string[] movs = File.ReadAllLines(archivoMov);
                // Corrección para compatibilidad: Reemplaza TakeLast(5) con Skip (disponible en .NET Framework 4.5+ y .NET Core 2.0+)
                int skipCount = Math.Max(0, movs.Length - 5);
                cuentaActual.Movimientos = movs.Skip(skipCount).ToList();
            }
        }


        public void Deposito()
        {
            Console.Write("Monto a depositar: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal monto) && monto > 0)
            {
                cuentaActual.Saldo += monto;
                cuentaActual.AgregarMovimiento("Deposito", monto);
                GuardarCuenta();
                GuardarMovimientos();
                Console.WriteLine($"Depósito exitoso. Nuevo saldo: {cuentaActual.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Monto inválido.");
            }
        }

    }







}
