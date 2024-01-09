using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLibrary
{
    public class RecursiveFunction : Function
    {
        public RecursiveFunction(string nombre, List<Variable> parametro, List<string> cuerpo) : base(nombre, parametro, cuerpo)
        {

        }

        public RecursiveFunction() : base()
        {

        }
    }
}
