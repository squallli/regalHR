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
    public class OutgoingModelFactory
    {
        public DataAccess DbAccess = new DataAccess();

        public OutgoingModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }


        public List<OutgoingEventModel> GetEventList(string EmpNo, string SDATE, string EDATE)
        {
            List<OutgoingEventModel> query = new List<OutgoingEventModel>();



            DataTable dt = DbAccess.ExecuteDataTable("SELECT *,Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,Convert(char(10),Convert(datetime,OutDate),20) as start,left(OutTime,2)+':'+right(OutTime,2) as StartTime,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat FROM gvOutgoing WHERE Status IN ('A','U','AM','UM') AND OutMan=@OutMan AND OutDate BETWEEN @SDATE AND @EDATE Order by OutDate,OutTime",
                new DbParameter[] {
                    DataAccess.CreateParameter("OutMan", DbType.String, EmpNo.ToString()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString()),
                }
            );



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string BgColor = "#E0F2F7";
                string IsEdits = "Y";


                if (dt.Rows[i]["Status"].ToString() == "UM" || dt.Rows[i]["Status"].ToString() == "AM")
                {
                    BgColor = "#ffe4e1";
                }




                //賦予此單據是否可編輯之狀態!
                DateTime Now = DateTime.Parse(DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"));
                DateTime vDate = DateTime.Parse(dt.Rows[i]["start"].ToString() + " " + dt.Rows[i]["StartTime"].ToString());

                if (vDate.Ticks < Now.Ticks)
                {
                    IsEdits = "N";
                }






                query.Add(new OutgoingEventModel()
                {
                    OutId = dt.Rows[i]["OutId"].ToString(),
                    OutDate = dt.Rows[i]["start"].ToString(),
                    OutTime = dt.Rows[i]["StartTime"].ToString(),
                    RecordMan = dt.Rows[i]["RecordManName"].ToString(),
                    OutMan = dt.Rows[i]["OutMan"].ToString(),
                    Location = dt.Rows[i]["Location"].ToString(),
                    CustomerName = dt.Rows[i]["CustomerName"].ToString(),
                    Status = dt.Rows[i]["Status"].ToString(),
                    UpdateDate = dt.Rows[i]["UpdateDateFormat"].ToString(),
                    UpdateTime = dt.Rows[i]["UpdateTime"].ToString(),
                    Company = dt.Rows[i]["Company"].ToString(),
                    OutDescription = dt.Rows[i]["OutDescription"].ToString(),
                    GoOutTime = dt.Rows[i]["GoOutTimeFormat"].ToString(),
                    id = dt.Rows[i]["OutId"].ToString(),
                    title = dt.Rows[i]["CustomerName"].ToString(),
                    //start = dt.Rows[i]["start"].ToString(),
                    //為了排序用
                    start = vDate.ToString("yyyy-MM-dd HH:mm"),
                    allDay = true,
                    self = true,
                    backgroundColor = BgColor,
                    IsEdit = IsEdits
                });
            }



            return query;
        }



        /// <summary>
        /// 是否已經出門 (已作廢)
        /// </summary>
        /// <param name="OutId">外出紀錄id</param>
        /// <param name="OutMan">外出人</param>
        /// <returns></returns>
        public bool IsGoOut(string OutId,string OutMan)
        {

            string sql = "SELECT ISNULL(GoOutTime,'') AS GoOutTime FROM gvOutgoing WHERE OutId=@OutId AND OutMan=@OutMan  ";

            DataTable dt = DbAccess.ExecuteDataTable(sql, new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, OutId.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutMan.ToString()),
                }
            );

            if (dt.Rows.Count == 0)
            {
                throw new Exception("系統查無單據");


            }
            else
            {
                if (dt.Rows[0]["GoOutTime"].ToString() == "")
                {
                    return false;//已出門
                }
                else
                {
                    return true;//已出門
                }
                
            }

        }



        public bool EditOutgoing(OutgoingEventModel Outgoing)
        {
            /* 停用 解除
            //判斷是否有實際出門時間
            if (IsGoOut(Outgoing.OutId.ToString(), Outgoing.OutMan.ToString()))
            {
                throw new Exception("已有實際出門時間..無法編輯!");
            }
            */



            //賦予更新時間
            string UpdateDate = DateTime.Now.ToString("yyyyMMdd");
            string UpdateTime = DateTime.Now.ToString("HHmmss");
            string CheckUpdateDateTime = DateTime.Now.AddMinutes(-10).ToString("yyyyMMddHHmm");


            

            if (Outgoing.Equipment == null || Outgoing.Equipment == "")
            {
                Outgoing.Equipment = "";
            }



            //源頭單據  準備壓回CheckUpdateDateTime
            OutgoingEventModel OutOrder = GetLastOutgoing(Outgoing.OutId, Outgoing.OutMan);





            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {



                DbAccess.ExecuteNonQuery("INSERT INTO Outgoing (OutId,OutDate,OutTime,RecordMan,OutMan,Location,CustomerName,Status,UpdateDate,UpdateTime,Company,OutDescription,GoOutTime,Equipment,CheckUpdateDateTime) VALUES (@OutId,@OutDate,@OutTime,@RecordMan,@OutMan,@Location,@CustomerName,@Status,@UpdateDate,@UpdateTime,@Company,@OutDescription,@GoOutTime,@Equipment,@CheckUpdateDateTime) ", objTrans,
                new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, Outgoing.OutId.ToString()),
                    DataAccess.CreateParameter("OutDate", DbType.String, Outgoing.OutDate.ToString()),
                    DataAccess.CreateParameter("OutTime", DbType.String, Outgoing.OutTime.ToString()),
                    DataAccess.CreateParameter("RecordMan", DbType.String,Outgoing.RecordMan.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, Outgoing.OutMan.ToString()),
                    DataAccess.CreateParameter("Location", DbType.String,Outgoing.Location.ToString()),
                    DataAccess.CreateParameter("CustomerName", DbType.String, Outgoing.CustomerName.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String,Outgoing.Status.ToString()),//U或UM
                    DataAccess.CreateParameter("UpdateDate", DbType.String, UpdateDate),
                    DataAccess.CreateParameter("UpdateTime", DbType.String, UpdateTime),
                    DataAccess.CreateParameter("Company", DbType.String, Outgoing.Company),
                    DataAccess.CreateParameter("OutDescription", DbType.String, Outgoing.OutDescription),
                    DataAccess.CreateParameter("GoOutTime", DbType.String, Outgoing.GoOutTime.ToString()),
                    DataAccess.CreateParameter("Equipment", DbType.String, Outgoing.Equipment.ToString()),
                    DataAccess.CreateParameter("CheckUpdateDateTime", DbType.String, CheckUpdateDateTime),
                    });





                string sql = "Update Outgoing SET CheckUpdateDateTime = @CheckUpdateDateTime WHERE OutId = @OutId AND OutMan = @OutMan AND UpdateDate = @UpdateDate  AND UpdateTime = @UpdateTime ";

                DbAccess.ExecuteNonQuery(sql, objTrans,
                new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, OutOrder.OutId.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutOrder.OutMan.ToString()),
                    DataAccess.CreateParameter("UpdateDate", DbType.String, OutOrder.UpdateDate),
                    DataAccess.CreateParameter("UpdateTime", DbType.String,  OutOrder.UpdateTime),
                    DataAccess.CreateParameter("CheckUpdateDateTime", DbType.String,  CheckUpdateDateTime),
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


        public bool DeleteOutgoing(OutgoingEventModel Outgoing)
        {

            /*解除 停用
            //判斷是否有實際出門時間
            if (IsGoOut(Outgoing.OutId.ToString(), Outgoing.OutMan.ToString()))
            {
                throw new Exception("已有實際出門時間..無法刪除!");
            }
            */

            if (Outgoing.Equipment == null || Outgoing.Equipment=="")
            {
                Outgoing.Equipment = "";
            }


            //源頭單據  準備壓回CheckUpdateDateTime
            OutgoingEventModel OutOrder = GetLastOutgoing(Outgoing.OutId,Outgoing.OutMan);




            //賦予更新時間
            string UpdateDate = DateTime.Now.ToString("yyyyMMdd");
            string UpdateTime = DateTime.Now.ToString("HHmmss");
            string CheckUpdateDateTime = DateTime.Now.AddMinutes(-10).ToString("yyyyMMddHHmm");

            
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {


                string sql = "INSERT INTO Outgoing (OutId,OutDate,OutTime,RecordMan,OutMan,Location,CustomerName,Company,GoOutTime,Status,UpdateDate,UpdateTime,Equipment,CheckUpdateDateTime) "
                           + " SELECT TOP 1 OutId,OutDate,OutTime,RecordMan,OutMan,Location,CustomerName,Company,GoOutTime,@Status,@UpdateDate,@UpdateTime,@Equipment,@CheckUpdateDateTime FROM gvOutgoing "
                           + " WHERE OutId = @OutId AND OutMan = @OutMan  Order by UpdateDate DESC,UpdateTime DESC ";
                    DbAccess.ExecuteNonQuery(sql, objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, Outgoing.OutId.ToString()),
                    DataAccess.CreateParameter("RecordMan", DbType.String,Outgoing.RecordMan.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, Outgoing.OutMan.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String,Outgoing.Status.ToString()),//D或DM
                    DataAccess.CreateParameter("UpdateDate", DbType.String, UpdateDate),
                    DataAccess.CreateParameter("UpdateTime", DbType.String,UpdateTime),
                    DataAccess.CreateParameter("Equipment", DbType.String,  Outgoing.Equipment),
                    DataAccess.CreateParameter("CheckUpdateDateTime", DbType.String,  CheckUpdateDateTime),
                    });



                    //找回上一筆外出單據  並押上 本次更新時間
                    sql = "Update Outgoing SET CheckUpdateDateTime = @CheckUpdateDateTime WHERE OutId = @OutId  AND OutMan = @OutMan AND UpdateDate = @UpdateDate  AND UpdateTime = @UpdateTime ";
                    DbAccess.ExecuteNonQuery(sql, objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, OutOrder.OutId.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutOrder.OutMan.ToString()),
                    DataAccess.CreateParameter("UpdateDate", DbType.String, OutOrder.UpdateDate),
                    DataAccess.CreateParameter("UpdateTime", DbType.String,  OutOrder.UpdateTime),
                    DataAccess.CreateParameter("CheckUpdateDateTime", DbType.String,  CheckUpdateDateTime),
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
        /// 新增出勤紀錄
        /// </summary>
        /// <param name="OutgoingForm"></param>
        /// <param name="OutgoingList"></param>
        /// <param name="OutgoingStatus">A或AM(補登)</param>
        /// <returns></returns>
        public bool InsertOutgoing(OutgoingFormModel OutgoingForm, List<OutgoingEventModel> OutgoingList,string OutgoingStatus)
        {





            //OutgoingList
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {



                DataTable dt = DbAccess.ExecuteDataTable("SELECT ISNULL(MAX(OutId),@NewId) as NewOutId FROM Outgoing WHERE OutMan=@OutMan AND OutId LIKE @OutId",
                new DbParameter[] {
                    DataAccess.CreateParameter("NewId", DbType.String,DateTime.Now.ToString("yyyyMMdd000")),
                    DataAccess.CreateParameter("OutId", DbType.String,DateTime.Now.ToString("yyyyMMdd")+"%"),
                    DataAccess.CreateParameter("OutMan", DbType.String,OutgoingForm.OutMan),
                }
                );


                decimal NewId = decimal.Parse(dt.Rows[0]["NewOutId"].ToString());
                NewId++;



                //賦予更新時間
                string UpdateDate =  DateTime.Now.ToString("yyyyMMdd");
                string UpdateTime = DateTime.Now.ToString("HHmmss");
                string CheckUpdateDateTime = DateTime.Now.AddMinutes(-10).ToString("yyyyMMddHHmm");


                for (int i = 0; i < OutgoingList.Count(); i++)
                {

                    if (OutgoingList[i].Equipment == null || OutgoingList[i].Equipment == "")
                    {
                        OutgoingList[i].Equipment = "";
                    }





                    DbAccess.ExecuteNonQuery("INSERT INTO Outgoing (OutId,OutDate,OutTime,RecordMan,OutMan,Location,CustomerName,Status,UpdateDate,UpdateTime,Company,OutDescription,GoOutTime,Equipment,CheckUpdateDateTime) VALUES (@OutId,@OutDate,@OutTime,@RecordMan,@OutMan,@Location,@CustomerName,@Status,@UpdateDate,@UpdateTime,@Company,@OutDescription,@GoOutTime,@Equipment,@CheckUpdateDateTime) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, NewId.ToString()),
                    DataAccess.CreateParameter("OutDate", DbType.String, OutgoingForm.SDate.ToString()),
                    DataAccess.CreateParameter("OutTime", DbType.String, OutgoingList[i].OutTime.ToString()),  //更改外出時間 
                    DataAccess.CreateParameter("RecordMan", DbType.String,OutgoingForm.RecordMan.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutgoingForm.OutMan.ToString()),
                    DataAccess.CreateParameter("Location", DbType.String,OutgoingList[i].Location.ToString()),
                    DataAccess.CreateParameter("CustomerName", DbType.String, OutgoingList[i].CustomerName.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String,OutgoingStatus.ToString()),//A或AM
                    DataAccess.CreateParameter("UpdateDate", DbType.String,UpdateDate),
                    DataAccess.CreateParameter("UpdateTime", DbType.String,  UpdateTime),
                    DataAccess.CreateParameter("Company", DbType.String, OutgoingForm.OutManCompany.ToString()),
                    DataAccess.CreateParameter("OutDescription", DbType.String, OutgoingForm.OutDescription.ToString()),
                    DataAccess.CreateParameter("GoOutTime", DbType.String, OutgoingList[i].GoOutTime.ToString()),
                    DataAccess.CreateParameter("Equipment", DbType.String, OutgoingList[i].Equipment.ToString()),
                    DataAccess.CreateParameter("CheckUpdateDateTime", DbType.String, CheckUpdateDateTime),
                    });
                    NewId++;
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
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvEmployeeAll WHERE 1 = 1 AND Status='1' " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpModel M = new EmpModel();
                M.EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString();
                M.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                M.EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString();
                M.DepartMentName = dt.Rows[i]["DepartMentName"].ToString();
                M.DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString();
                M.CardNo = dt.Rows[i]["CardNo"].ToString();
                M.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                DataList.Add(M);
            }





            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }



       
        public EmpModel GetOutMan(string EmpNo)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvEmployeeAll Where EmployeeNo=@EmpNo", new DbParameter[] {
                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo.ToString())
                }
            );

            EmpModel Emp = null;
            if(dt.Rows.Count!=0){
                Emp = new EmpModel();
                Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
                Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
                Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();
                Emp.Company = dt.Rows[0]["Company"].ToString();
                Emp.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                Emp.DepartMentNo = dt.Rows[0]["DepartMentNo"].ToString();
                Emp.DepartMentName = dt.Rows[0]["DepartMentName"].ToString();
            }

            return Emp;
        }







        public List<OutgoingEventModel> GetOutHistory(string OutId, string OutMan)
        {
            List<OutgoingEventModel> DataList = new List<OutgoingEventModel>();
            string Sql = " SELECT *,Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,Convert(char(10),Convert(datetime,OutDate),20) as start,left(OutTime,2)+':'+right(OutTime,2) as StartTime,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat FROM gvOutHistory WHERE OutId=@OutId AND OutMan=@OutMan ";
            DataTable dt = DbAccess.ExecuteDataTable(Sql, new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, OutId),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutMan)
                }
            );

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                OutgoingEventModel M = new OutgoingEventModel();

                M.UpdateDate = dt.Rows[i]["UpdateDateFormat"].ToString();
                M.start = dt.Rows[i]["start"].ToString() + " " + dt.Rows[i]["StartTime"].ToString();
                M.GoOutTime = dt.Rows[i]["GoOutTimeFormat"].ToString();
                M.OutId = dt.Rows[i]["OutId"].ToString();
                M.OutMan = dt.Rows[i]["OutMan"].ToString();
                M.OutTime = dt.Rows[i]["OutTime"].ToString();
                M.Location = dt.Rows[i]["Location"].ToString();
                M.CustomerName = dt.Rows[i]["CustomerName"].ToString();
                M.Status = dt.Rows[i]["Status"].ToString().Trim();
                M.RecordMan = dt.Rows[i]["RecordManName"].ToString();
                M.OutDescription = dt.Rows[i]["OutDescription"].ToString();
                M.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                DataList.Add(M);
            }

            return DataList;
        }




        public void GetHistoryViewList(string Where, string Orderby, int PageNum, int Limit, ref List<OutgoingEventModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;



            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,*,Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,Convert(char(10),Convert(datetime,OutDate),20) as start,left(OutTime,2)+':'+right(OutTime,2) as StartTime,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat  FROM gvOutgoing WHERE 1 = 1 " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                OutgoingEventModel M = new OutgoingEventModel();
                
                M.UpdateDate = dt.Rows[i]["UpdateDateFormat"].ToString();
                M.start = dt.Rows[i]["start"].ToString() + " " + dt.Rows[i]["StartTime"].ToString();
                M.GoOutTime = dt.Rows[i]["GoOutTimeFormat"].ToString();
                M.OutId = dt.Rows[i]["OutId"].ToString();
                M.OutMan = dt.Rows[i]["OutMan"].ToString();
                M.OutTime = dt.Rows[i]["OutTime"].ToString();
                M.Location = dt.Rows[i]["Location"].ToString();
                M.CustomerName = dt.Rows[i]["CustomerName"].ToString();
                M.Status = dt.Rows[i]["Status"].ToString().Trim();
                M.RecordMan = dt.Rows[i]["RecordManName"].ToString();
                M.OutDescription = dt.Rows[i]["OutDescription"].ToString();
                M.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                
                DataList.Add(M);
            }





            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }



        /// <summary>
        /// 取得該人的該張單據的最後一筆 外出單據 (重點是要取得UpdateDate  UpdateTime)
        /// </summary>
        /// <param name="OutMan">外出人</param>
        /// <param name="OutId">外出單號</param>
        public OutgoingEventModel GetLastOutgoing(string OutId, string OutMan)
        {
            

            //只會取得一筆資料  因為使用的table是 gvOutgoing
            string Sql = "SELECT OutId,OutMan,UpdateDate,UpdateTime from gvOutgoing WHERE OutId=@OutId AND OutMan=@OutMan ";
            DataTable dt = DbAccess.ExecuteDataTable(Sql, new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, OutId),
                    DataAccess.CreateParameter("OutMan", DbType.String, OutMan)
                }
            );





            if (dt.Rows.Count==0)
            {
                throw new Exception("查無外出單據");
            }

            OutgoingEventModel Data = new OutgoingEventModel();

            Data.OutId = dt.Rows[0]["OutId"].ToString();
            Data.OutMan = dt.Rows[0]["OutMan"].ToString();
            Data.UpdateDate = dt.Rows[0]["UpdateDate"].ToString();
            Data.UpdateTime = dt.Rows[0]["UpdateTime"].ToString();

            return Data;
        }

    }
}