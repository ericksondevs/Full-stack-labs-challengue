using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitOfWork.Helpers
{
    public class UrlShortener
    {
        private const string Alphabet = "BCDFGHJKLMNPQRSTVWXYZ";
        private static readonly int Base = Alphabet.Length;

        public static string Encode(int num)
        {
            if (num == 0) return Alphabet[0].ToString();
            num = num * 500000;
            var s = string.Empty;

            while (num > 0)
            {
                s += Alphabet[num % Base];
                num = num / Base;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static int Decode(string str)
        {
            var num = 0;
            for (var i = 0; i < str.Length; i++)
            {
                num = num * Base + Alphabet.IndexOf(str.ElementAt(i));
            }
            return num;
        }
    }
}