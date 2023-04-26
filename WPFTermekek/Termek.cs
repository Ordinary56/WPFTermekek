using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFTermekek
{
    public class Termek 
    {
        public string? kategoria { get; }
        public string? gyarto { get; }
        public string? nev { get; }
        public int? Ar { get; }
        public int? Garido { get; }

        public Termek(string? k,string? g,string? n,int? ar,int? garido)
        {
            kategoria = k;
            gyarto = g;
            nev = n;
            Ar = ar;
            Garido = garido;
        }

        
    }
}
