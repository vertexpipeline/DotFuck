using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] a;
            a = new int[30000];
            a[12] = 33;
            int c = a[12];
        }
    }
}
