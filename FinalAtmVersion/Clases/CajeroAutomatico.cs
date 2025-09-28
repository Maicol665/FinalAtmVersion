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

    public void Retiro()
        {
            Console.Write("Monto a retirar: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal monto) && monto > 0 && monto <= cuentaActual.Saldo)
            {
                cuentaActual.Saldo -= monto;
                cuentaActual.AgregarMovimiento("Retiro", monto);
                GuardarCuenta();
                GuardarMovimientos();
                Console.WriteLine($"Retiro exitoso. Nuevo saldo: {cuentaActual.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Monto inválido o saldo insuficiente.");
            }
        }


        public void ConsultaSaldo()
        {
            Console.WriteLine($"Saldo actual: {cuentaActual.Saldo:C}");
        }

        /// <summary>
        /// e. Muestra los últimos 5 movimientos.
        /// </summary>
        public void ConsultaMovimientos()
        {
            Console.WriteLine("Últimos 5 movimientos:");
            if (cuentaActual.Movimientos.Count == 0)
            {
                Console.WriteLine("No hay movimientos.");
            }
            else
            {
                foreach (string mov in cuentaActual.Movimientos)
                {
                    Console.WriteLine(mov);
                }
            }
        }

        public void CambioClave()
        {
            Console.Write("Nuevo PIN (4 dígitos): ");
            string nuevoPin = Console.ReadLine();
            if (nuevoPin.Length == 4)
            {
                cuentaActual.Pin = nuevoPin;
                GuardarCuenta();
                Console.WriteLine("PIN cambiado exitosamente.");
            }
            else
            {
                Console.WriteLine("PIN debe tener 4 dígitos.");
            }
        }


        private void GuardarCuenta()
        {
            string[] lineas = File.ReadAllLines(ArchivoCuentas);
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] partes = lineas[i].Split(':');
                if (partes.Length == 3 && int.Parse(partes[0]) == cuentaActual.NumeroCuenta)
                {
                    lineas[i] = $"{cuentaActual.NumeroCuenta}:{cuentaActual.Pin}:{cuentaActual.Saldo}";
                    break;
                }
            }
            File.WriteAllLines(ArchivoCuentas, lineas);
        }

        private void GuardarMovimientos()
        {
            string archivoMov = string.Format(ArchivoMovimientos, cuentaActual.NumeroCuenta);
            File.WriteAllLines(archivoMov, cuentaActual.Movimientos);
        }

        public void MostrarMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== Cajero Automático ===");
                Console.WriteLine("1. Depósito");
                Console.WriteLine("2. Retiro");
                Console.WriteLine("3. Consulta Saldo");
                Console.WriteLine("4. Últimos 5 Movimientos");
                Console.WriteLine("5. Cambio de Clave");
                Console.WriteLine("0. Salir");
                Console.Write("Opción: ");

                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        Deposito();
                        break;
                    case "2":
                        Retiro();
                        break;
                    case "3":
                        ConsultaSaldo();
                        break;
                    case "4":
                        ConsultaMovimientos();
                        break;
                    case "5":
                        CambioClave();
                        break;
                    case "0":
                        Console.WriteLine("Gracias por usar el cajero.");
                        return;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }



    }
}