using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightProperties
{
    class Option
    {
        private static String s_DIRECTORY;

        /// <summary>
        /// Parse arguments
        /// </summary>
        /// <param name="_args">Arguments to parse</param>
        /// <returns>True if all arguments a valid</returns>
        public static bool parse(String[] _args)
        {
            bool isValid = true;
            if(_args.Length <= 0)
            {
                isValid = false;
            }
            else if(_args.Length > 1)
            {
                isValid = false;
            }

            if (!isValid)
            {
                Console.WriteLine("The directory name is needed");
                return false;
            }

            s_DIRECTORY = _args[0];

            if (!Directory.Exists(Option.getDirectory()))
            {
                Console.WriteLine("Directory `" + s_DIRECTORY + "` does not exist");
                return false;
            }

            return true;
        }

        public static String getDirectory()
        {
            return s_DIRECTORY;
        }
    }
}
