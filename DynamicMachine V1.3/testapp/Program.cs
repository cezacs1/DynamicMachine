using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testapp
{
    class Program
    {
        static void Main()
        {
            int val = valueadder(13, 90);
            val += seksen;
            val += __200();
            val += __50();
            val += intparse("999");
            val += (int)testulong(5);

            Console.WriteLine(val);
            Console.ReadKey();
        }

        static ulong testulong(int value)
        {
            return 1500 * 9 / 7;
        }


        public static int intparse(string input)
        {
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int valueadder(int a, int b)
        {
            int c = a + b;
            return c;
        }

        public static int __50()
        {
            int s = 0;
            for (int i = 0; i < 5; i++)
                s += 10;
            return s;
        }

        public static int seksen = 80;
        private static int __200()
        {
            int p = valueadder(seksen, 31);
            return p == 100 ? 200 : 100;
        }

    }
}
