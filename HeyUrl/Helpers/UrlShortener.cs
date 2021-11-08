using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyUrl.Helpers
{
    public class UrlShortener
    {
        private const string Alphabet = "BCDFGHJKLMNPQRSTVWXYZ";
        private static readonly int Base = Alphabet.Length;

        public static string Encode(int num)
        {
          
            var sb = new StringBuilder();
            while (sb.Length < 5)
            {
                sb.Insert(0, Alphabet.ElementAt(num % Base));
                num = num / Base;
            }
            return sb.ToString();
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