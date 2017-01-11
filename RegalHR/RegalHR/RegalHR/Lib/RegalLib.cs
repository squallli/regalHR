using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using RegalCustEncrypt;
using System.IO;
using System.Text;

namespace RegalHR
{
    public class RegalLib
    {


        /// <summary>
        /// 資料庫連線字串
        /// </summary>
        public static string DbConnStr
        {
            get
            {

                /*
                string DATABASE_IP = System.Configuration.ConfigurationManager.AppSettings["Data Source"];  //GetConfig("DATABASE", "DATABASE_IP", "Conn.ini");
                string DATABASE_NAME = System.Configuration.ConfigurationManager.AppSettings["Database"]; //GetConfig("DATABASE", "DATABASE_NAME", "Conn.ini");
                string DATABASE_USER = System.Configuration.ConfigurationManager.AppSettings["User Id"]; //GetConfig("DATABASE", "DATABASE_USER", "Conn.ini");
                string DATABASE_PASSWORD = System.Configuration.ConfigurationManager.AppSettings["Password"]; //GetConfig("DATABASE", "DATABASE_PASSWORD", "Conn.ini");
                

                return "Data Source="+DATABASE_IP+";Initial Catalog="+DATABASE_NAME+";User ID="+DATABASE_USER+";password="+DATABASE_PASSWORD;
                */

                return System.Configuration.ConfigurationManager.AppSettings["connectionStr"];
            }
        }

        /// <summary>
        /// 資料庫連線ProviderName
        /// </summary>
        public static string ProviderName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        /// <summary>
        /// 記錄Log檔案
        /// </summary>
        /// <param name="Msg">訊息</param>
        /// <param name="SaveFileName">檔名</param>
        public static void LogOutput(string Msg, string SaveFileName = "")
        {
            string SaveFilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\" + SaveFileName+"_"+DateTime.Now.ToString("yyyyMMdd")+".log";

            if (File.Exists(SaveFilePath) == false)
            {
                StreamWriter sw = new StreamWriter(SaveFilePath);
                sw.Close();
            }

            string MsgDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Msg = "[" + MsgDateTime + "] " + Msg + "\r\n";

            File.AppendAllText(SaveFilePath, Msg, Encoding.Unicode);

        }

        /// <summary>
        /// 回傳ini的設定值
        /// </summary>
        /// <param name="session">歸類head</param>
        /// <param name="key">Value欄位</param>
        /// <param name="IniPath">INI位置</param>
        /// <returns></returns>
        public static string GetConfig(string session, string key,string IniFile)
        {
            string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + IniFile;

            StreamReader sr = new StreamReader(IniFilePath, Encoding.Default);
            string line = "", head = "", result = "";
            string[] value;
            char[] spChr = { '=' };

            while ((line = sr.ReadLine()) != null)
            {
                if (line.IndexOf("[") != -1 && line.IndexOf("]") != -1)
                {
                    head = line.Replace("[", "").Replace("]", "");
                    continue;
                }
                else
                {
                    if (head == session)
                    {
                        value = line.Split(spChr);
                        if (value[0] == key)
                        {
                            result = value[1];
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            sr.Close();
            line = null;
            head = null;
            value = null;
            spChr = null;
            sr = null;
            return result;
        }
    }
}