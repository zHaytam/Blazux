using System;
using System.Drawing;

namespace PlainSample
{
    public static class Utils
    {

        private static readonly Random _rnd = new Random();

        public static string GetRandomColor()
        {
            var c = Color.FromArgb(_rnd.Next(256), _rnd.Next(256), _rnd.Next(256));
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

    }
}
