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
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var fullPath = Path.GetFullPath(@".");
                GrepFile.DirWalk(fullPath);
            }
            else
            {
                foreach (string path in args)
                {
                    var fullPath = Path.GetFullPath(path);
                    GrepFile.DirWalk(fullPath);
                }
            }
        }
    }
}
