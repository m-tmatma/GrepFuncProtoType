using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GrepFuncProtoType
{
    /// <summary>
    /// プロトタイプ宣言で型名しかない箇所を Grep するクラス
    /// </summary>
    public class GrepFile
    {
        const string strReturn    = @"\b(?<return>\w+)\b";
        const string strFunc      = @"(?<func>[A-Za-z]\w*)";  // 先頭はアルファベット
        const string strType      = @"(?<type>[A-Za-z]\w*)";  // 先頭はアルファベット
        const string strConst     = @"(\bconst\b)?";
        const string strAnd       = @"&";
        const string strPointer   = @"\*";
        const string strPointerOption = @"\*?";
        const string strAndOrPtr  = @"\s*(" + strAnd + @"|" + strPointer + @")*\s*";
        const string strParam     = strConst + strType + strAndOrPtr;
        const string strParams    = @"(?<params>[^\)]+)";
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
            strConst,
            strPointerOption,
            strReturn,
            strPointerOption,
            strFunc,
            strLeftPar,
            strParams,
            strRightPar,
            strConst,
            strSemicolon,
        };
        static readonly char[] comma = new char[] {
            ','
        };

        /// <summary>
        /// rep() で使用する正規表現の文字列
        /// </summary>
        static readonly string strString = string.Join(strSpaceOpt, elements);

        /// <summary>
        /// Grep() で使用する正規表現クラス
        /// </summary>
        static Regex regPrototype = new Regex(strString);

        /// <summary>
        /// パラメータの要素を分解する正規表現クラス
        /// </summary>
        static Regex regParams = new Regex(@"\s+");

        /// <summary>
        /// 関数の引数の '(' ～ ')' までの間のデータを解析する
        /// </summary>
        /// <param name="parameters">'(' ～ ')' までの間のデータ</param>
        /// <returns>対象とするデータの場合 true を返す</returns>
        public static bool CheckParams(string parameters)
        {
            // 先頭、末尾の空白を削除する
            string clippedParameters = parameters.Trim();

            // カンマで分離する
            string[] parametersElement = clippedParameters.Split(comma);

            // void 関数の場合は問題ない
            if (parametersElement.Length == 1 && parametersElement[0].Trim() == "void")
            {
                return false;
            }

            // 分離したパラメータごとに処理する
            foreach(string parameter in parametersElement)
            {
                // パラメータの要素を分離して解析する
                string[] elements = regParams.Split(parameter.Trim());
                if (elements.Length == 1)
                {
                    // 要素数が一つの場合、タイプはないはず。でも完全ではない。
                    return true;
                }
           }
            return false;
        }

        /// <summary>
        /// 指定したファイルからプロトタイプ宣言で型名しかない箇所を標準出力に出す。入力ファイルは UTF-8 と仮定する
        /// </summary>
        /// <param name="fileName">入力ファイル名</param>
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
                        string valFunc = match.Groups["func"].Value;
                        string valParams = match.Groups["params"].Value;

                        if (Array.IndexOf(keywords, valReturn) >= 0)
                        {
                            continue;
                        }
                        if (Array.IndexOf(keywords, valFunc) >= 0)
                        {
                            continue;
                        }

                        if (!CheckParams(valParams))
                        {
#if DEBUG
                            // うまく動かなかった場合のデバッグ用
                            CheckParams(valParams);
#endif
                            continue;
                        }
                        Console.WriteLine(fileName + "(" + lineNumber.ToString() + ")" + ":" + match.Groups[0].Value);
                    }
                }
            }
        }

        /// <summary>
        /// ヘッダファイルのみ grep の対象とするのでフィルタリングして grep を実施する
        /// </summary>
        /// <param name="fileName"></param>
        private static void ProcessFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (ext == ".h")
            {
                Grep(fileName);
            }
        }

        /// <summary>
        /// フォルダを再帰的にたどってサブフォルダにあるヘッダファイルを grep する。
        /// ファイルのパスを指定すれば指定したファイルのみ対象とする。
        /// </summary>
        /// <param name="targetPath">探す対象のフォルダまたはファイルパス</param>
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
                        // 再帰呼び出し
                        // サブフォルダから検索する。
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
