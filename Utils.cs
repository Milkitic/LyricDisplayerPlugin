﻿using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public static class Utils
    {
        private static void OutputToSyncIO(string message, ConsoleColor color, bool new_line = true, bool time = true)
        {
            IO.CurrentIO.WriteColor("[LyricDisplayer]" + message, color, new_line, time);
        }

        private static void OutputToLocalConsole(string message, ConsoleColor color, bool new_line = true, bool time = true)
        {
            Console.ForegroundColor = color;
            Console.Write((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : string.Empty)
               + message
               + (new_line ? Environment.NewLine : string.Empty));
            Console.ResetColor();
        }

        public static void Output(string message, ConsoleColor color, bool new_line = true, bool time = true)
        {
            if (Setting.IsUsedByPlugin)
                OutputToSyncIO(message, color, new_line, time);
            else
                OutputToLocalConsole(message, color, new_line, time);
        }


        public static void Debug(string message, bool new_line = true, bool time = true)
        {
            if (Setting.DebugMode)
                Output(message, ConsoleColor.Cyan, new_line, time);
        }

        //https://www.programcreek.com/2013/12/edit-distance-in-java/
        public static int EditDistance(string a, string b)
        {
            int len_a = a.Length;
            int len_b = b.Length;

            int[,] dp = new int[len_a + 1, len_b + 1];

            for (int i = 0; i <= len_a; i++)
            {
                dp[i, 0] = i;
            }

            for (int j = 0; j <= len_b; j++)
            {
                dp[0, j] = j;
            }

            for (int i = 0; i < len_a; i++)
            {
                char c_a = a[i];
                for (int j = 0; j < len_b; j++)
                {
                    char c_b = b[j];

                    if (c_a == c_b)
                    {
                        dp[i + 1, j + 1] = dp[i, j];
                    }
                    else
                    {
                        int replace = dp[i, j] + 1;
                        int insert = dp[i, j + 1] + 1;
                        int delete = dp[i + 1, j] + 1;

                        int min = Math.Min(Math.Min(insert, replace), delete);
                        dp[i + 1, j + 1] = min;
                    }
                }
            }

            return dp[len_a, len_b];
        }

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

    }
}
