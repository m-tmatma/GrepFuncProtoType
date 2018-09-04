using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepFuncProtoType
{
    public class GrepFile
    {
        public static void DirWalk(string targetDir)
        {
            try
            {
                foreach (string file in Directory.GetFiles(targetDir))
                {
                    Console.WriteLine(file);
                }
                foreach (string dir in Directory.GetDirectories(targetDir))
                {
                    DirWalk(dir);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
