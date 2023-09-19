using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    public class Class1
    {
        public static void Main()
        {

            Console.WriteLine(0 + 1);
            Console.WriteLine(4 + 3);

            /*
            int a = 1;
            int b = 2;
            Console.WriteLine(a + b);
            */

            Console.WriteLine(10);

            Console.WriteLine("amk");

            Method1();

            Console.WriteLine("2. yöntem çağrılmadan önce bu konsol yazısı var.");

            newvoid();

            Console.WriteLine("3. yöntem (int) çağrılmadan önce bu konsol yazısı var.");

            int besyuz = newint();

            Console.WriteLine(besyuz);
            
        }

        public static void Method1()
        {
            Console.WriteLine("Method1 called");
        }

        public static void newvoid()
        {
            Console.WriteLine("newvoid called");
        }

        public static int newint()
        {
            return 500;
        }
    }
}
