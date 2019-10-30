using System;
using System.IO;

namespace SightProperties
{
    class Option
    {
        private static string s_DIRECTORY;

        /// <summary>
        /// Parse arguments
        /// </summary>
        /// <param name="_args">Arguments to parse</param>
        /// <returns>True if all arguments a valid</returns>
        public static bool parse(string[] _args)
        {
            bool isValid = true;
            if (_args.Length <= 0)
            {
                isValid = false;
            }
            else if (_args.Length > 1)
            {
                isValid = false;
            }

            if (!isValid)
            {
                Console.WriteLine("The directory name is needed");
                return false;
            }

            s_DIRECTORY = _args[0];

            if (!Directory.Exists(s_DIRECTORY))
            {
                Console.WriteLine("Directory `" + s_DIRECTORY + "` does not exist");
                return false;
            }

            return true;
        }

        public static bool isPropertiesDirectory()
        {
            return File.Exists(s_DIRECTORY + "\\Properties.cmake");
        }

        public static string getDirectory()
        {
            return s_DIRECTORY;
        }
    }
}
