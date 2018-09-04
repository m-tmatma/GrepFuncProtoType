﻿using System;
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
        const string strFunc      = @"(?<func>\w+)";
        const string strParam     = @"(?<param>\w+)";
        const string strLeftPar   = @"\(";
        const string strRightPar  = @"\)";
        const string strSemicolon = @";";
        const string strSpace = @"\s*";
        static readonly string[] elements = new string[] {
            strReturn,
            strFunc,
            strLeftPar,
            strParam,
            "(,",
            strParam,
            ")*",
            strRightPar,
            strSemicolon,
        };
        static readonly string strString = string.Join(strSpace, elements);
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
                        Console.WriteLine(fileName + "(" + lineNumber.ToString() + ")" + ":" + match.Groups[0].Value);
                    }
                }
            }
        }

        public static void DirWalk(string targetDir)
        {
            try
            {
                foreach (string file in Directory.GetFiles(targetDir))
                {
                    var ext = Path.GetExtension(file);
                    if (ext == ".h" || ext == ".cpp")
                    {
                        Grep(file);
                    }
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