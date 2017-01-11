using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;

namespace RegalHR.ModelFactory
{
    public class HolidayModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public HolidayModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }











        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<HolidayModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;



            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvHoliday WHERE 1 = 1 " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum > " + offset;


            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HolidayModel M = new HolidayModel();
                M.Company = dt.Rows[i]["Company"].ToString();
                M.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                M.YearId = dt.Rows[i]["YearId"].ToString();
                M.EditPK = dt.Rows[i]["PK"].ToString();
                DataList.Add(M);
            }


            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }





        public List<HolidayDtlModel> GetEventList(string Company,string YearId, string SDATE, string EDATE)
        {
            List<HolidayDtlModel> query = new List<HolidayDtlModel>();



            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvHolidayDtl WHERE Company = @Company AND YearId = @YearId  AND Holiday BETWEEN @SDATE AND @EDATE Order by Holiday ",
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                    DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString()),
                }
            );



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string BgColor = "#E0F2F7";
                string IsEdits = "Y";

                string title = dt.Rows[i]["CtypeName"].ToString()+" \n "+dt.Rows[i]["Memo"].ToString();

                
                if (dt.Rows[i]["Display"].ToString() == "1" )
                {
                    if (dt.Rows[i]["Ctype"].ToString() == "0")
                    {
                        BgColor = "#00BFFF";
                    }
                    else
                    {
                        BgColor = "#ffe4e1";
                    }
                    
                }




                

                query.Add(new HolidayDtlModel()
                {
                    Company = dt.Rows[i]["Company"].ToString(),
                    id = dt.Rows[i]["Holiday"].ToString(),
                    title = title,
                    start = dt.Rows[i]["start"].ToString(),
                    allDay = true,
                    self = true,
                    Memo = dt.Rows[i]["Memo"].ToString(),
                    YearId = dt.Rows[i]["YearId"].ToString(),
                    Ctype = dt.Rows[i]["Ctype"].ToString(),
                    CtypeName = dt.Rows[i]["CtypeName"].ToString(),
                    Display =  dt.Rows[i]["Display"].ToString(),
                    DisplayName = dt.Rows[i]["DisplayName"].ToString(),
                    backgroundColor = BgColor,
                });


            }



            return query;
        }



        /// <summary>
        /// 刪除日歷
        /// </summary>
        /// <param name="Company">公司別</param>
        /// <param name="YearId">年份</param>
        /// <returns></returns>
        public bool DeleteHoliday(string Company, string YearId)
        {


            DbTransaction objTrans = DbAccess.CreateDbTransaction();

            try
            {


                DbAccess.ExecuteNonQuery("DELETE FROM  tbHoliday WHERE YearId=@YearId AND Company=@Company ", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                });



                DbAccess.ExecuteNonQuery("DELETE FROM  tbHolidayDtl WHERE YearId=@YearId AND Company=@Company", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                });



                objTrans.Commit();
                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }

        }



        /// <summary>
        /// 新增日歷
        /// </summary>
        /// <param name="Company">公司別</param>
        /// <param name="YearId">年份</param>
        /// <returns></returns>
        public bool InsertHoliday(string Company,string YearId)
        {
            



            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbHoliday WHERE Company = @Company AND YearId = @YearId  ",
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                    DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                }
            );

            if (dt.Rows.Count != 0)
            {
                throw new Exception("行事曆已存在!!");
            }

            DbTransaction objTrans = DbAccess.CreateDbTransaction();
            try
            {








                DbAccess.ExecuteNonQuery("INSERT INTO tbHoliday (YearId,Company) VALUES (@YearId,@Company) ", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                });


                

                DateTime ThisDate= DateTime.Parse(YearId + "-01-01");
                DateTime EndDate= DateTime.Parse(YearId + "-12-31");

                for (; ThisDate <= EndDate; ThisDate=ThisDate.AddDays(1))
                {

                    if (ThisDate.DayOfWeek == DayOfWeek.Sunday || ThisDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        //假如是  六日 則新增資料
                        DbAccess.ExecuteNonQuery("INSERT INTO tbHolidayDtl (YearId,Company,Ctype,Holiday,Display,Memo) VALUES (@YearId,@Company,@Ctype,@Holiday,@Display,@Memo) ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                        DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                        DataAccess.CreateParameter("Ctype", DbType.String, "1"),
                        DataAccess.CreateParameter("Holiday", DbType.String, ThisDate.ToString("yyyyMMdd")),
                        DataAccess.CreateParameter("Display", DbType.String,"0"),
                        DataAccess.CreateParameter("Memo", DbType.String,""),
                        });
                    }
                }



                objTrans.Commit();
                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }

        }







        /// <summary>
        ///  複製行事曆
        /// </summary>
        /// <param name="Company">公司別</param>
        /// <param name="YearId">年份</param>
        /// <param name="CopyId">複製至公司別</param>
        /// <returns></returns>
        public bool CopyHoliday(string Company, string YearId,string CopyId)
        {




            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbHoliday WHERE Company = @Company AND YearId = @YearId  ",
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, CopyId.ToString()),
                    DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                }
            );

            if (dt.Rows.Count != 0)
            {
                throw new Exception("行事曆已存在!!");
            }

            DbTransaction objTrans = DbAccess.CreateDbTransaction();



            try
            {


                DbAccess.ExecuteNonQuery("INSERT INTO tbHoliday (YearId,Company) VALUES (@YearId,@Company) ", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, CopyId.ToString()),
                });



                DbAccess.ExecuteNonQuery("INSERT INTO tbHolidayDtl (YearId,Company,Ctype,Holiday,Display,Memo) SELECT YearId,@CopyId,Ctype,Holiday,Display,Memo FROM tbHolidayDtl WHERE Company = @Company AND YearId = @YearId ", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                DataAccess.CreateParameter("CopyId", DbType.String, CopyId.ToString()),
                });



                objTrans.Commit();
                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }

        }





        public bool InsertHolidayDtl(HolidayDtlModel HolidayDtl)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {




                DbAccess.ExecuteNonQuery("INSERT INTO tbHolidayDtl (YearId,Company,Ctype,Holiday,Display,Memo) VALUES (@YearId,@Company,@Ctype,@Holiday,@Display,@Memo) ", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("YearId", DbType.String, HolidayDtl.YearId.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, HolidayDtl.Company.ToString()),
                DataAccess.CreateParameter("Ctype", DbType.String, HolidayDtl.Ctype.ToString()),
                DataAccess.CreateParameter("Holiday", DbType.String,HolidayDtl.Holiday.ToString()),
                DataAccess.CreateParameter("Display", DbType.String,HolidayDtl.Display.ToString()),
                DataAccess.CreateParameter("Memo", DbType.String,HolidayDtl.Memo.ToString()),
                });



                objTrans.Commit();

                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }

        }





        public bool EditHolidayDtl(HolidayDtlModel HolidayDtl)
        {
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {



                DbAccess.ExecuteNonQuery("UPDATE  tbHolidayDtl SET Ctype=@Ctype,Display=@Display,Memo=@Memo WHERE YearId=@YearId AND Company=@Company AND Holiday=@Holiday ", objTrans,
                new DbParameter[] {
                    DataAccess.CreateParameter("Ctype", DbType.String, HolidayDtl.Ctype.ToString()),
                    DataAccess.CreateParameter("Display", DbType.String, HolidayDtl.Display.ToString()),
                    DataAccess.CreateParameter("Memo", DbType.String, HolidayDtl.Memo.ToString()),

                    DataAccess.CreateParameter("YearId", DbType.String, HolidayDtl.YearId.ToString()),
                    DataAccess.CreateParameter("Company", DbType.String, HolidayDtl.Company.ToString()),
                    DataAccess.CreateParameter("Holiday", DbType.String, HolidayDtl.id.ToString()), //使用ID 來取代
                    });

                objTrans.Commit();
                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }
        }





        public bool DeleteHolidayDtl(HolidayDtlModel HolidayDtl)
        {
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {


                string sql = "DELETE FROM tbHolidayDtl WHERE YearId=@YearId AND Company=@Company AND Holiday=@Holiday  ";
                DbAccess.ExecuteNonQuery(sql, objTrans,
                new DbParameter[] {
                    DataAccess.CreateParameter("YearId", DbType.String, HolidayDtl.YearId.ToString()),
                    DataAccess.CreateParameter("Company", DbType.String, HolidayDtl.Company.ToString()),
                    DataAccess.CreateParameter("Holiday", DbType.String, HolidayDtl.id.ToString()), //使用ID 來取代

                    });

                objTrans.Commit();
                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }
        }


        public HolidayModel GetHolidayData(string PK)
        {
            HolidayModel Holiday=null;


            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvHoliday Where PK=@PK",
                new DbParameter[] {
                    DataAccess.CreateParameter("PK", DbType.String,  PK)
                }
            );


            if(dt.Rows.Count > 0)
            {

                Holiday = new HolidayModel();
                Holiday.EditPK = PK;
                Holiday.YearId = dt.Rows[0]["YearId"].ToString();
                Holiday.Company = dt.Rows[0]["Company"].ToString();
                Holiday.CompanyName = dt.Rows[0]["CompanyName"].ToString();

            }
            return Holiday;
        }


    }
}