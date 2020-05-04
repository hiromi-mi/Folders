using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Rottytooth.Esolang.Folders.Runtime;

namespace Rottytooth.Esolang.Folders
{
    public class Program
    {
        const string ERROR_STRING = "Could not compile.\nERRORS:\n\n";

        const string HOW_TO_FOLDER =
            "USAGE: Folders [options] path_to_root_folder\n\nOptions include:/sTranspile to C# and output the C# rather than building\n/eCreate an exe, rather than running the code immediately";

        internal static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine(ERROR_STRING + "no path provided");
                return;
            }

            string path;
            string[] arguments = GetOptions(args, out path);

            // If /b option is provided, use "basic" or "concise" or "classic" folders interpreting

            // Basic Folders is now legacy
            bool pureFolders = !(arguments.Contains("b"));

            bool exe = !(arguments.Contains("e"));


            // If /s option is provided, put code out as C# string
            if (arguments.Contains("s"))
            {
                ProgramBuilder builder = ProgramBuilder.Generate(pureFolders, path);
                builder.BuildProgram();
                Console.Write(builder.ProgramText);
                return;
            }

            // Otherwise, compile the program

            string errors = "";

            bool succeeded = Compile(path, ref errors, exe, pureFolders);

            Console.WriteLine(); // clear line after program output
            if (succeeded)
            {
                Console.Error.WriteLine("Complete");
            }
            else
            {
                Console.Error.Write(ERROR_STRING);
                Console.Error.WriteLine(errors);
            }
        }

        public static string[] GetOptions(string[] args, out string path)
        {
            var arguments = new List<string>();
            string outpath = "";

            foreach(string arg in args)
            {
                if (arg[0] == '/' || arg[0] == '-')
                {
                    arguments.Add(arg.Substring(1));
                }
                else
                {
                    outpath = arg;
                }
            }

            path = outpath;
            return arguments.ToArray();
        }

        /// <summary>
        /// Actually build the Folders program
        /// </summary>
        /// <param name="path">path to root directory of the Folders program</param>
        /// <param name="errors">used to return error messages</param>
        /// <param name="exe">whether we create an exe or simply compile + run in memory</param>
        /// <param name="pureFolders">Pure Folders (non-semantic folder names) vs the (legacy) Classic Folders Syntax</param>
        /// <returns></returns>
        public static bool Compile(string path, ref string errors, bool exe, bool pureFolders)
        {
            ProgramBuilder builder = ProgramBuilder.Generate(pureFolders, path);
            builder.BuildProgram();

            StringBuilder errorList = new StringBuilder();

            string entireProgram =
                "#include <bits/stdc++.h>\n" + builder.Declarations + "int main() {\n" + builder.ProgramText + "\n}";

            Console.WriteLine(entireProgram);


            if (errorList.Length > 0)
            {
                errors = errorList.ToString();
                return false;
            }

            // we have successfully compiled
            return true;
        }
    }
}
