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
            GrepFile.DirWalk(@".");
        }
    }
}
