using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepFuncProtoType
{
    class Program
    {
        static readonly string[] defaultPaths = { @"." };

        static void Main(string[] args)
        {
            string[] files = defaultPaths;
            if (args.Length > 0)
            {
                files = args;
            }
            foreach (string path in files)
            {
                var fullPath = Path.GetFullPath(path);
                GrepFile.DirWalk(fullPath);
            }
        }
    }
}
