using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GrepFuncProtoType
{
    public class GrepFile
    {
        const string strReturn    = @"(?<return>\w+)";
        const string strFunc      = @"(?<func>[A-Za-z]\w*)";  // 先頭はアルファベット
        const string strParam     = @"(?<param>[A-Za-z]\w*)"; // 先頭はアルファベット
        const string strLeftPar   = @"\(";
        const string strRightPar  = @"\)";
        const string strSemicolon = @";";
        const string strSpaceOpt  = @"\s*";	// 0個以上の空白
        const string strSpace     = @"\s+";	// 少なくとも一つ以上空白が必要
        static readonly string[] keywords = {
            "if",
            "for",
            "while",
            "do",
            "switch",
            "return",
            "goto",
        };

        static readonly string[] elements = new string[] {
            "^",
            strReturn + strSpace,
            strFunc,
            strLeftPar,
            strParam,
            "(,",
            strParam,
            ")*",
            strRightPar,
            strSemicolon,
        };
        static readonly string strString = string.Join(strSpaceOpt, elements);
        static Regex regPrototype = new Regex(strString);

        public static void Grep(string fileName)
        {
            using (var streamReader = new StreamReader(fileName, Encoding.GetEncoding("utf-8")))
            {
                int lineNumber = 0;
                string line = string.Empty;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineNumber++;

                    var match = regPrototype.Match(line);
                    if (match.Success)
                    {
                        string valReturn = match.Groups["return"].Value;
                        string valFunc   = match.Groups["func"].Value;
                        string valParam  = match.Groups["param"].Value;

                        if (Array.IndexOf(keywords, valReturn) >= 0)
                        {
                            continue;
                        }
                        if (Array.IndexOf(keywords, valFunc) >= 0)
                        {
                            continue;
                        }
                        if (Array.IndexOf(keywords, valParam) >= 0)
                        {
                            continue;
                        }
                        if (valParam == "void")
                        {
                            // 関数の引数が void の場合は問題ない
                            continue;
                        }
                        Console.WriteLine(fileName + "(" + lineNumber.ToString() + ")" + ":" + match.Groups[0].Value);
                    }
                }
            }
        }

        private static void ProcessFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (ext == ".h")
            {
                Grep(fileName);
            }
        }

        public static void DirWalk(string targetPath)
        {
            try
            {
                if(File.Exists(targetPath))
                {
                    ProcessFile(targetPath);
                }
                else if(Directory.Exists(targetPath))
                {
                    foreach (string file in Directory.GetFiles(targetPath))
                    {
                        ProcessFile(file);
                    }
                    foreach (string dir in Directory.GetDirectories(targetPath))
                    {
                        DirWalk(dir);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
