using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryBot
{
    class Utils
    {
        public static string StringBuilder(string message)
        {
            var clean = message.ToLower();
            var words = clean.Split(' ');

            // Check the first word.
            if (words[0].StartsWith("<@"))
                clean = String.Join(" ", words.Skip(1));

            // Check the last word
            if (words[words.Length - 1].StartsWith("<@"))
                clean = String.Join(" ", words.Take(words.Length - 1));

            return clean;
        }
    }
}
