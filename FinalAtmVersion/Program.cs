using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAtmVersion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CajeroAutomatico cajero = new CajeroAutomatico();
            cajero.Inicializar();

            while (true)
            {
                if (cajero.IniciarSesion())
                {
                    cajero.MostrarMenu();
                }
                else
                {
                    Console.WriteLine("Presione Enter para intentar de nuevo o Ctrl+C para salir.");
                    Console.ReadLine();
                }


            }
    }
}
