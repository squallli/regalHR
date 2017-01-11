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
    public class EmpModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public EmpModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }




        /// <summary>
        /// 新增Emp資料
        /// </summary>
        /// <param name="Program">ProgramModel</param>
        /// <returns></returns>
        public bool EmpAdd(EmpModel Emp)
        {

            //禁止新增資料 , 改由鼎新Table同步!
            return false;

            /*
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            DataTable ChkCard = DbAccess.ExecuteDataTable("SELECT Count(*) AS Total FROM tbCard WHERE CardNo = @CardNo AND  Status='1' ",
                    new DbParameter[] {
                     DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString())
                     }
            );


            if (ChkCard.Rows[0]["Total"].ToString() != "0")
            {
                throw new Exception("卡號重覆!");
            }



            try
            {
                DbAccess.ExecuteNonQuery("INSERT INTO tbEmployee(EmployeeNo,EmployeeName,DepartMentNo,dayofduty,Status) VALUES (@EmployeeNo,@EmployeeName,@DepartMentNo,@dayofduty,@Status) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("EmployeeName", DbType.String, Emp.EmployeeName.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, Emp.DepartMentNo.ToString()),
                    DataAccess.CreateParameter("dayofduty", DbType.String, Emp.dayofduty.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String, "1"),
                    });



                if (Emp.CardNo != "")
                {
                    DbAccess.ExecuteNonQuery("INSERT INTO tbCard(EmployeeNo,CardNo,Status,UpdateDate) VALUES (@EmployeeNo,@CardNo,@Status,@UpdateDate) ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                        DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
                        DataAccess.CreateParameter("Status", DbType.String, "1"),
                        DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    });
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
            */
        }











        /// <summary>
        /// 編輯Emp資料
        /// </summary>
        /// <param name="Program">ProgramModel</param>
        /// <returns></returns>
        public bool EmpEdit(EmpModel Emp)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {


                //當此員工有卡號時 就得更新..卡號有效日
                if (Emp.CardStatus == "1")
                {
                    DbAccess.ExecuteNonQuery("UPDATE tbCard SET EffectiveDate=@EffectiveDate,ExpiryDate=@ExpiryDate WHERE EmployeeNo = @EditPK AND CardNo = @CardNo AND Status = '1' ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("EditPK", DbType.String, Emp.EditPK.ToString()),
                        DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
                        DataAccess.CreateParameter("EffectiveDate", DbType.String, Emp.CardEffectiveDate.ToString()),
                        DataAccess.CreateParameter("ExpiryDate", DbType.String, Emp.CardExpiryDate.ToString()),
                        });
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






        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<EmpModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;

            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,*,"
                  +" Convert(char(10),dayofdutyDT,120) as dayofdutyFormat, "
                  +" CASE 	WHEN Status = 1 THEN ''	WHEN Status = 0 THEN Convert(char(10),offDutyDate,120) END AS offDutyDateFormat, "
                  + " CASE Status	WHEN 1 THEN DATEDIFF(day,dayofdutyDT,GETDATE())	WHEN 2 THEN DATEDIFF(day,dayofdutyDT,GETDATE())	 ELSE DATEDIFF(day,dayofdutyDT,Convert(char(10),offDutyDate,120))  END AS WorkDuration, "
                  + " CASE Sex WHEN  '1' THEN '男' WHEN '2' THEN '女'  END AS SexFormat FROM gvEmployee "
                  +" WHERE 1 = 1  " + Where;




            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            string EmpNoStr = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpModel M = new EmpModel();
                M.Company = dt.Rows[i]["Company"].ToString();
                M.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                M.EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString();
                M.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                M.EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString();
                M.DepartMentName = dt.Rows[i]["DepartMentName"].ToString();
                M.DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString();
                M.dayofduty = dt.Rows[i]["dayofdutyFormat"].ToString();
                M.CardNo = dt.Rows[i]["CardNo"].ToString();
                M.WorkDuration = dt.Rows[i]["WorkDuration"].ToString();
                M.offDutyDate = dt.Rows[i]["offDutyDateFormat"].ToString();
                M.Sex = dt.Rows[i]["SexFormat"].ToString();
                DataList.Add(M);
                EmpNoStr +="'"+M.EmployeeNo +"',";

            }




            if (dt.Rows.Count != 0)
            {


                EmpNoStr=EmpNoStr.TrimEnd(',');
                Sql = "SELECT EmployeeNo,GroupName,ViewLevel,ViewLevelName FROM tbEmpGroup EG LEFT OUTER JOIN gvGroup GP ON EG.GroupID=GP.GroupID  WHERE EmployeeNo IN ( " + EmpNoStr + " ) ";

                DataTable dt2 = DbAccess.ExecuteDataTable(Sql);

                for (int i = 0; i < DataList.Count; i++)
                {
                    DataRow[] DR = dt2.Select(" EmployeeNo = '" + DataList[i].EmployeeNo + "' ");


                    for (int j = 0; j < DR.Count();j++ )
                    {
                        GroupModel Tmp =  new GroupModel();
                        Tmp.GroupName  = DR[j]["GroupName"].ToString();

                        Tmp.ViewLevelName  = DR[j]["ViewLevelName"].ToString();
                        Tmp.ViewLevel = DR[j]["ViewLevel"].ToString();



                            
                        DataList[i].Group.Add(Tmp);
                    }
                }

            }







            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }




        /// <summary>
        /// 列出該公司部門底下之員工 (下拉式選單用)
        /// </summary>
        /// <param name="FulltimeFlag">1= 全職  0=臨時 空=全部</param>
        /// /// <param name="Sex">性別 1= 男生 2=女生</param>
        /// <param name="Company">公司別</param>
        /// <param name="DepartMentNo">部門別</param>
        /// <param name="status">狀態 : 1=使用中  0=離職</param>
        /// <returns></returns>
        public List<EmpModel> GetEmpList(string FulltimeFlag,string Sex,string Company, string DepartMentNo, string status)
        {

            List<EmpModel> EmpList = new List<EmpModel>();

            string sql = "SELECT * FROM gvEmployeeAll WHERE 1 = 1";


            if (Company != "")
            {
                sql += " AND Company = '" + Company + "' ";
            }

            if (DepartMentNo != "")
            {
                sql += " AND DepartMentNo = '" + DepartMentNo + "' ";
            }

            if (status != "")
            {
                sql += " AND Status = '" + status + "' ";
            }


            if (FulltimeFlag != "")
            {
                sql += " AND FulltimeFlag = '" + FulltimeFlag + "' ";
            }




            if (Sex != "")
            {
                sql += " AND Sex = '" + Sex + "' ";
            }




            DataTable dt = DbAccess.ExecuteDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpList.Add(new EmpModel()
                {
                    EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString(),
                });
            }

            return EmpList;


        }








        public List<DepartMentModel> GetDepartMentList()
        {
            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbDepartMent ORDER BY DepartMentNo ");

            List<DepartMentModel> DepartMentList = new List<DepartMentModel>();


            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                DepartMentList.Add(new DepartMentModel()
                {
                    DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString(),
                    DepartMentName = dt.Rows[i]["DepartMentName"].ToString()
                });

            }

            return DepartMentList;
            
        }





        public EmpModel GetEmpData(string EmployeeNo)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT *,Convert(char(10),dayofdutyDT,120) as dayofdutyFormat FROM gvEmployee Where EmployeeNo = @EmployeeNo ",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, EmployeeNo.ToString())
                }
            );

            EmpModel Emp = null;

            
            if(dt.Rows.Count==0)
            {
                throw new Exception("查無資料");
            }
            else if(dt.Rows.Count>=2)
            {
                throw new Exception("有多筆資料");
            }
            else
            {
                Emp = new EmpModel();

                Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
                Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
                Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();
                Emp.DepartMentName = dt.Rows[0]["DepartMentName"].ToString();
                Emp.DepartMentNo = dt.Rows[0]["DepartMentNo"].ToString();
                Emp.dayofduty = dt.Rows[0]["dayofdutyFormat"].ToString();
                Emp.Company = dt.Rows[0]["Company"].ToString();
                Emp.CompanyName = dt.Rows[0]["CompanyName"].ToString();

                Emp.EditPK = Emp.EmployeeNo;//修改專用PK值

              





                DataTable dt2 = DbAccess.ExecuteDataTable("SELECT CardNo,Convert(char(10),Convert(datetime,EffectiveDate),120) AS EffectiveDateFormat,Convert(char(10),Convert(datetime,ExpiryDate),120) AS ExpiryDateFormat FROM tbCard WHERE EmployeeNo = @EmployeeNo AND  Status='1' ",
                    new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString())
                    }
                );


                if( dt2.Rows.Count==0 )
                {
                    Emp.CardStatus = "0"; 
                    Emp.CardNo = null; 
                }
                else if (dt2.Rows.Count == 1)
                {
                    Emp.CardStatus = "1"; 
                    Emp.CardNo = dt2.Rows[0]["CardNo"].ToString();
                    Emp.CardExpiryDate = dt2.Rows[0]["ExpiryDateFormat"].ToString();
                    Emp.CardEffectiveDate = dt2.Rows[0]["EffectiveDateFormat"].ToString();
                }
                else
                {
                    throw new Exception("有多筆卡號");
                }
            }

            return Emp;
        }



        public bool InsertCardNo(EmpModel Emp, string NewCardNo)
        {


            DataTable ChkDt = DbAccess.ExecuteDataTable("SELECT Count(*) AS TOTAL FROM tbCard WHERE CardNo = @CardNo AND Status = '1' ",
                new DbParameter[] {
                                    DataAccess.CreateParameter("CardNo", DbType.String, NewCardNo)
                                }
            );

            if (ChkDt.Rows[0]["TOTAL"].ToString() != "0")
            {
                throw new Exception("重覆卡號!");
            }



            DbTransaction objTrans = DbAccess.CreateDbTransaction();

            try
            {
                //檢查CardNo是否有重覆&使用中









                DbAccess.ExecuteNonQuery("INSERT INTO tbCard(EmployeeNo,CardNo,Status,UpdateDate,EffectiveDate,ExpiryDate) VALUES(@EmployeeNo,@CardNo,@Status,@UpdateDate,@EffectiveDate,@ExpiryDate)", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("CardNo", DbType.String, NewCardNo),
                    DataAccess.CreateParameter("Status", DbType.String, "1"),
                    DataAccess.CreateParameter("EffectiveDate", DbType.String, DateTime.Now.ToString("yyyyMMdd")),
                    DataAccess.CreateParameter("ExpiryDate", DbType.String, "99991231"),
                    DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
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
        /// 註銷卡號
        /// </summary>
        /// <param name="EmployeeNo"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool DeleteCardNo(EmpModel Emp)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {
                DbAccess.ExecuteNonQuery("UPDATE tbCard SET Status='0',UpdateDate=@UpdateDate WHERE EmployeeNo = @EmployeeNo AND CardNo=@CardNo AND Status = '1' ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
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

        








    }
}