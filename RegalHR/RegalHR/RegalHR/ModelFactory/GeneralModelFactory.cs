using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;
using Iteedee.ApkReader;
using System.IO;

namespace RegalHR.ModelFactory
{
    public class GeneralModelFactory
    {

        public static List<ItemModel> GetTempEmpStatusList()
        {
            List<ItemModel> ItemList = new List<ItemModel>();

            ItemList.Add(new ItemModel()
            {
                ItemID = "1",
                ItemName = "使用中"
            });


            ItemList.Add(new ItemModel()
            {
                ItemID = "0",
                ItemName = "已註銷"
            });


            return ItemList;
        }







        public static List<ItemModel> GetEmpSexList()
        {
            List<ItemModel> ItemList = new List<ItemModel>();



            ItemList.Add(new ItemModel()
            {
                ItemID = "1",
                ItemName = "男"
            });


            ItemList.Add(new ItemModel()
            {
                ItemID = "2",
                ItemName = "女"
            });


            return ItemList;
        }




        public static List<ItemModel> GetEmpStatusList()
        {
            List<ItemModel> ItemList = new List<ItemModel>();



            ItemList.Add(new ItemModel()
            {
                ItemID = "1",
                ItemName = "在職"
            });


            ItemList.Add(new ItemModel()
            {
                ItemID = "0",
                ItemName = "離職"
            });


            return ItemList;
        }




        public static List<ItemModel> GetGroupViewLevelList()
        {
            List<ItemModel> ItemList = new List<ItemModel>();

            ItemList.Add(new ItemModel()
            {
                ItemID = "A",
                ItemName = "企業級"
            });


            ItemList.Add(new ItemModel()
            {
                ItemID = "B",
                ItemName = "公司級"
            });


            ItemList.Add(new ItemModel()
            {
                ItemID = "C",
                ItemName = "部門級"
            });


            return ItemList;
        }


        public static List<CompanyModel> GetCompanyList(string Company)
        {

            DataAccess DbAccess = new DataAccess();
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;


            List<CompanyModel> CompanyList = new List<CompanyModel>();


            string sql = "SELECT * FROM tbCompany order by CompanyName";

            if (Company != "")
            {
                sql = " SELECT * FROM tbCompany where Company = '" + Company + "' order by CompanyName";
            }

            DataTable dt = DbAccess.ExecuteDataTable(sql);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CompanyList.Add(new CompanyModel()
                {
                    Company = dt.Rows[i]["Company"].ToString().Trim(),
                    CompanyName = dt.Rows[i]["CompanyName"].ToString().Trim()
                });
            }

            return CompanyList;
        }






        public static List<DepartMentModel> GetDepList(string Company,string DepNo)
        {

            DataAccess DbAccess = new DataAccess();
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;


            List<DepartMentModel> DepartMentList = new List<DepartMentModel>();

            string sql = "SELECT * FROM tbDepartMent ";


            if (Company != "")
            {
                if(DepNo !="")
                {
                    sql = " SELECT * FROM tbDepartMent where Company = '" + Company + "' AND DepartMentNo= '" + DepNo + "'  order by DepartMentName";
                }
                else
                {
                    sql = " SELECT * FROM tbDepartMent where Company = '" + Company + "'  order by DepartMentName";
                }
            }


            DataTable dt = DbAccess.ExecuteDataTable(sql);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DepartMentList.Add(new DepartMentModel()
                {
                    Company = dt.Rows[i]["Company"].ToString().Trim(),
                    DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString().Trim(),
                    DepartMentName = dt.Rows[i]["DepartMentName"].ToString().Trim(),

                });
            }

            return DepartMentList;
        }











        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckProgID(List<ProgramModel> ProgramList,string ProgID)
        {
            int query = (from Data in ProgramList
                         where Data.ProgID == ProgID
                         select Data
                     ).Count();

            if (query == 0)
            {
                return false;
            }else{
                return true ;
            }
        }





        /// <summary>
        /// 判定是否為日期格式
        /// </summary>
        /// <param name="DATE"></param>
        /// <returns></returns>
        public bool CheckDate(string DATE)
        {
            bool Result = true;
            DateTime SDATE;

            if (!DateTime.TryParse(DATE, out SDATE))
            {
                return false;
            }

            return Result;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SearchCompanyDefault(UserModel User,ProgramModel Program)
        {
            if (Program.ViewLevel == "A")
            {
                return "";
            }
            else
            {
                return User.Company;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SearchDepartMentDefault(UserModel User, ProgramModel Program)
        {
            if (Program.ViewLevel == "A" || Program.ViewLevel == "B")
            {
                return "";
            }
            else
            {
                return User.DepNo;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProgramModel GetProgram(List<ProgramModel> ProgramList, string ProgID)
        {
            ProgramModel query = null;
            try
            {
                 query = (from Data in ProgramList
                                      where Data.ProgID == ProgID
                                      select Data).First();
            }
            catch
            {

            }

            return query;
        }



        /// <summary>
        /// 所有GridViewer每頁顯示幾筆
        /// </summary>
        public static string Version
        {
            get
            {
                #if DEBUG
                     return DateTime.Now.ToString("yyyyMMddHHmmss");//隨意亂數值  主要不讓chrome cache 住 css,js
                #else
                     return "2016091202";
                #endif
            }
        }



        /// <summary>
        /// 所有GridViewer每頁顯示幾筆
        /// </summary>
        public static string WebTitle
        {
            get
            {
                return "帝商科技-出勤系統";

            }
        }





        /// <summary>
        /// 所有GridViewer每頁顯示幾筆
        /// </summary>
        public int GridViewerLimit
        {
            get
            {
                return 10;
            }
        }




        /// <summary>
        /// 所有GridViewer每頁顯示幾筆
        /// </summary>
        public int AttendanceLimit
        {
            get
            {
                return 50;
            }
        }













        public static string getAPKVersion(string path)
        {
            byte[] manifestData = null;
            byte[] resourcesData = null;
            using (ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(File.OpenRead(path)))
            {
                using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    ICSharpCode.SharpZipLib.Zip.ZipFile zipfile = new ICSharpCode.SharpZipLib.Zip.ZipFile(filestream);
                    ICSharpCode.SharpZipLib.Zip.ZipEntry item;

                    while ((item = zip.GetNextEntry()) != null)
                    {
                        if (item.Name.ToLower() == "androidmanifest.xml")
                        {
                            manifestData = new byte[50 * 1024];
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                strm.Read(manifestData, 0, manifestData.Length);
                            }

                        }
                        if (item.Name.ToLower() == "resources.arsc")
                        {
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                using (BinaryReader s = new BinaryReader(strm))
                                {
                                    resourcesData = s.ReadBytes((int)s.BaseStream.Length);

                                }
                            }
                        }
                    }
                }
            }

            ApkReader apkReader = new ApkReader();
            return apkReader.getVersion(manifestData, resourcesData);
        }

    }





}