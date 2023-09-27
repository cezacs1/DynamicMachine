﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMachine
{
    public static class Undetected
    {
        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < 2; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            for (int i = 2; i < 6; i++)
            {
                stringChars[i] = numbers[random.Next(numbers.Length)];
            }
            for (int i = 6; i < 8; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        private static Random random = new Random(); //STRONG RENAMER CANNOT BE DECOMPILED BY "DE4DOT"
        public static string RandomStringStrong(int length)
        {
            const string chars = "埃克斯留艾儿波留豆大波"/*"0123456789EFACDB"/*//**"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"*/;
            string amk = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return /*"_" + */amk;
        }
    }
}
