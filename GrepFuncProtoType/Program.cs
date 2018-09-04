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
            //var basePath = @".";
            //var targetPath = @".";
            //var fi = new System.IO.FileInfo(Path.Combine(basePath, targetPath));
            //GrepFile.DirWalk(fi.FullName);

            var fullPath = Path.GetFullPath(@".");
            GrepFile.DirWalk(fullPath);
        }
    }
}
