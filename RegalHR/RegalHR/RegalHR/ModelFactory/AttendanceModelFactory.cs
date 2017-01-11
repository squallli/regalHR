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
    public class AttendanceModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public AttendanceModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }

        /// <summary>
        /// 輸入部門編號帶出部門底下之員工
        /// </summary>
        /// <param name="DepartMentNo">部門編號</param>
        /// <returns></returns>
        public List<EmpModel> GetEmpAllList(string DepartMentNo)
        {
            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvEmployeeAll Where DepartMentNo=@DepartMentNo AND Status='1'  ORDER BY DepartMentNo ", new DbParameter[] {
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, DepartMentNo.ToString())
                }
            );


            List<EmpModel> EmpList = new List<EmpModel>();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpList.Add(new EmpModel()
                {
                    EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString()
                });

            }

            return EmpList;
        }




        public DataTable GetAttendanceDataTable(int ManageFlag, string Company, string DepNo, string EmpNo, string KeyWord, DateTime SDATE, DateTime EDATE, string EmpStatus, int LimitVal, int SkipVal)
        {
            TimeSpan TS = new TimeSpan(EDATE.Ticks - SDATE.Ticks);
            int DiffDay = TS.Days;

            if (Math.Abs(DiffDay) > 366)
            {
                throw new Exception("僅能查詢日期區間一年內的資料!");
            }

            //DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbTemp ORDER BY EmployeeNo,WorkDay");

            DataTable dt = DbAccess.ExecuteDataTable("EXEC SearchAttendanceTemp @ManageFlag,@Company,@DepNo,@EmpNo,@KeyWord,@SDATE,@EDATE,@EmpStatus,@LimitVal,@SkipVal",
                new DbParameter[] {
                    DataAccess.CreateParameter("ManageFlag", DbType.Int16, ManageFlag),
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString().Trim()),
                    DataAccess.CreateParameter("DepNo", DbType.String, DepNo.ToString().Trim()),
                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo.ToString().Trim()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString("yyyy-MM-dd").ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString("yyyy-MM-dd").ToString()),

                    DataAccess.CreateParameter("EmpStatus", DbType.String, EmpStatus),
                    DataAccess.CreateParameter("LimitVal", DbType.Int32, LimitVal),
                    DataAccess.CreateParameter("SkipVal", DbType.Int32, SkipVal),

                    DataAccess.CreateParameter("KeyWord", DbType.String, KeyWord)
                }
            );
            return dt;
        }

        public List<AttendanceModel> GetAttendanceList(int ManageFlag, string Company, string DepNo, string EmpNo, string KeyWord, DateTime SDATE, DateTime EDATE, string EmpStatus, int LimitVal, int SkipVal)
        {

            TimeSpan TS = new TimeSpan(EDATE.Ticks - SDATE.Ticks);
            int DiffDay = TS.Days;

            if (Math.Abs(DiffDay) > 366)
            {
                throw new Exception("僅能查詢日期區間一年內的資料!");
            }


            DataTable dt = DbAccess.ExecuteDataTable("EXEC SearchAttendanceTemp @ManageFlag,@Company,@DepNo,@EmpNo,@KeyWord,@SDATE,@EDATE,@EmpStatus,@LimitVal,@SkipVal",
                new DbParameter[] {
                    DataAccess.CreateParameter("ManageFlag", DbType.Int16, ManageFlag),
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString().Trim()),
                    DataAccess.CreateParameter("DepNo", DbType.String, DepNo.ToString().Trim()),
                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo.ToString().Trim()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString("yyyy-MM-dd").ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString("yyyy-MM-dd").ToString()),

                    DataAccess.CreateParameter("EmpStatus", DbType.String, EmpStatus),
                    DataAccess.CreateParameter("LimitVal", DbType.Int32, LimitVal),
                    DataAccess.CreateParameter("SkipVal", DbType.Int32, SkipVal),


                    DataAccess.CreateParameter("KeyWord", DbType.String, KeyWord)
                }
            );

            List<AttendanceModel> AttendanceList = new List<AttendanceModel>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AttendanceList.Add(new AttendanceModel()
                {
                    EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString(),
                    EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString(),
                    CompanyName = dt.Rows[i]["CompanyName"].ToString(),
                    DepartMentName = dt.Rows[i]["DepartMentName"].ToString(),
                    StartWorkTime = dt.Rows[i]["StartWorkTime"].ToString(),
                    EndWorkTime = dt.Rows[i]["EndWorkTime"].ToString(),
                    WorkDay = dt.Rows[i]["WorkDay"].ToString(),
                    LateMin = dt.Rows[i]["LateMin"].ToString(),
                    LeaveMin = dt.Rows[i]["LeaveMin"].ToString(),
                    StartWorkOvertime = dt.Rows[i]["StartWorkOvertime"].ToString(),
                    EndWorkOvertime = dt.Rows[i]["EndWorkOvertime"].ToString(),
                    AttendanceDesc = dt.Rows[i]["AttendanceDesc"].ToString(),
                    AttendanceDescM = dt.Rows[i]["AttendanceDescM"].ToString(),
                    AttendanceDescN = dt.Rows[i]["AttendanceDescN"].ToString(),
                    AttendanceDescOT = dt.Rows[i]["AttendanceDescOT"].ToString(),
                    LateMinFormat = dt.Rows[i]["LateMinFormat"].ToString(),
                    AttendanceDesc2 = dt.Rows[i]["AttendanceDesc2"].ToString(),

                    RecordLimit = dt.Rows[i]["RecordLimit"].ToString(),
                    RowNumID = dt.Rows[i]["RowNumID"].ToString(),
                    RowNumTotal = dt.Rows[i]["RowNumTotal"].ToString(),
                    
                });

            }
            return AttendanceList;
        }





        public List<AttendanceModel> AttendanceRecord(string EmpNo, string TempEmpNo,string SDATE)
        {
            List<AttendanceModel> AttendanceList = new List<AttendanceModel>();



            DataTable dt = DbAccess.ExecuteDataTable("SELECT CheckInFlag,CheckInEmployeeName,CheckInDescription,SUBSTRING ( CheckInDate, 1 ,4 )+ '-'+ SUBSTRING (CheckInDate , 5, 2 )+'-' + SUBSTRING( CheckInDate ,7 , 2) AS CheckInDate,SUBSTRING ( CardDate, 1 ,4 )+ '-'+ SUBSTRING (CardDate , 5, 2 )+'-' + SUBSTRING( CardDate ,7 , 2) AS CardDate, SUBSTRING (CardTime , 1, 2 )+':' + SUBSTRING( CardTime ,3 , 2) as CardTime,CASE CardType WHEN 1 THEN '上班'WHEN 2 THEN '下班'WHEN 3 THEN '加班上班'WHEN 4 THEN '加班下班' ELSE '無法判定' END AS CardTypeName  FROM tbAttendance WHERE EmployeeNo IN (@EmpNo,@TempEmpNo) AND CardDate = @SDATE ORDER BY CardTime",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo.ToString().Trim()),
                    DataAccess.CreateParameter("TempEmpNo", DbType.String, TempEmpNo.ToString().Trim()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE)
                }
            );


 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AttendanceList.Add(new AttendanceModel()
                {
                    
                    CardTypeName = dt.Rows[i]["CardTypeName"].ToString(),
                    StartWorkTime = dt.Rows[i]["CardDate"].ToString() + " " + dt.Rows[i]["CardTime"].ToString(),
                    CheckInEmployeeName = dt.Rows[i]["CheckInEmployeeName"].ToString(),
                    CheckInFlag = dt.Rows[i]["CheckInFlag"].ToString(),
                    CheckInDescription = dt.Rows[i]["CheckInDescription"].ToString(),

                    CheckInDate = dt.Rows[i]["CheckInDate"].ToString(),
                });
            }

            return AttendanceList;
        }






        public HolidayDtlModel GetThisDate(string Company,string SDATE)
        {
            HolidayDtlModel Holiday  = new HolidayDtlModel();

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvHolidayDtl WHERE Company=@Company AND Holiday=@SDATE",
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString().Trim()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString().Trim())
                }
            );

            if (dt.Rows.Count!=0)
            {
                Holiday.title = dt.Rows[0]["CtypeName"].ToString();

                if (dt.Rows[0]["Memo"].ToString().Trim() != "")
                {
                    Holiday.title += " (" + dt.Rows[0]["Memo"].ToString() + ")";
                }
            }
            else
            {
                Holiday.title = "上班日";
            }
            return Holiday;
        }


        public EmpModel GetEmpAllData(string EmployeeNo)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvEmployeeAll  Where EmployeeNo = @EmployeeNo ",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, EmployeeNo.ToString())
                }
            );

            EmpModel Emp = null;

            if (dt.Rows.Count!=0)
            {
                Emp = new EmpModel();

                Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
                Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
                Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();
                Emp.DepartMentName = dt.Rows[0]["DepartMentName"].ToString();
                Emp.DepartMentNo = dt.Rows[0]["DepartMentNo"].ToString();
                Emp.Company = dt.Rows[0]["Company"].ToString();
                Emp.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                Emp.TempEmployeeNo = "";


                //找到對應的臨時員工編號
                DataTable dt2 = DbAccess.ExecuteDataTable("SELECT EmployeeNo FROM tbTempEmployee Where FullTimeEmployeeNo = @FullTimeEmployeeNo ",
                new DbParameter[] {
                                    DataAccess.CreateParameter("FullTimeEmployeeNo", DbType.String, EmployeeNo.ToString())
                                }
                );


                if (dt2.Rows.Count!=0)
                {
                    Emp.TempEmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
                }
            }
            return Emp;
        }

        public AttendanceShowDtlModel GetAttendanceShowDtl(string SDATE,string EmpNo)
        {
            string thisDate = SDATE;
            SDATE = SDATE.Replace("-", ""); 
            
            AttendanceShowDtlModel Obj = new AttendanceShowDtlModel();


            Obj.Emp = GetEmpAllData(EmpNo);

            Obj.ThisDate = GetThisDate(Obj.Emp.Company, SDATE);
            Obj.ThisDate.start = thisDate;

            OutgoingModelFactory OutgoingFactory = new OutgoingModelFactory();
            Obj.OutgoingList = OutgoingFactory.GetEventList(EmpNo, SDATE, SDATE);
 
            Obj.AttendanceList = AttendanceRecord(Obj.Emp.EmployeeNo, Obj.Emp.TempEmployeeNo, SDATE);

            return Obj;

        }


        

        public AttendanceCheckInDtlModel GetAttendanceCheckInDtl(string SDATE, string EmpNo)
        {

            AttendanceCheckInDtlModel Obj = new AttendanceCheckInDtlModel();

            Obj.Emp = GetEmpAllData(EmpNo);
            Obj.CardDate = SDATE;
            Obj.CardType = "1";
            Obj.CardTime = DateTime.Now.ToString("HH:mm");
            Obj.CheckInDescription = "";//補登
            return Obj;

        }


        /// <summary>
        /// 補登出勤紀錄
        /// </summary>
        /// <returns></returns>
        public bool SaveCheckInDtlForm(AttendanceCheckInDtlModel AttendanceCheckInDtlForm)
        {
            //OutgoingList
            DbTransaction objTrans = DbAccess.CreateDbTransaction();

            try
            {
                DbAccess.ExecuteNonQuery("INSERT INTO tbAttendance (EmployeeNo,Company,EmployeeName,EnEmployeeName,DepartMentName,CardType,WriteDate,WriteTime,CardDate,CardTime,CheckInFlag,CheckInEmployeeNo,CheckInDate,CheckInTime,CheckInEmployeeName,CardNo,CheckInDescription) VALUES(@EmployeeNo,@Company,@EmployeeName,@EnEmployeeName,@DepartMentName,@CardType,@WriteDate,@WriteTime,@CardDate,@CardTime,@CheckInFlag,@CheckInEmployeeNo,@CheckInDate,@CheckInTime,@CheckInEmployeeName,@CardNo,@CheckInDescription)", objTrans,
                new DbParameter[] {
                DataAccess.CreateParameter("EmployeeNo", DbType.String, AttendanceCheckInDtlForm.Emp.EmployeeNo.ToString()),
                DataAccess.CreateParameter("Company", DbType.String, AttendanceCheckInDtlForm.Emp.Company.ToString()),
                DataAccess.CreateParameter("EmployeeName", DbType.String, AttendanceCheckInDtlForm.Emp.EmployeeName.ToString()),
                DataAccess.CreateParameter("EnEmployeeName", DbType.String, AttendanceCheckInDtlForm.Emp.EmployeeEName.ToString()),
                DataAccess.CreateParameter("DepartMentName", DbType.String, AttendanceCheckInDtlForm.Emp.DepartMentName.ToString()),
                DataAccess.CreateParameter("CardType", DbType.Int16,AttendanceCheckInDtlForm.CardType.ToString()),
                DataAccess.CreateParameter("WriteDate", DbType.String,AttendanceCheckInDtlForm.CheckInDate.ToString()),
                DataAccess.CreateParameter("WriteTime", DbType.String,AttendanceCheckInDtlForm.CheckInTime.ToString()),
                DataAccess.CreateParameter("CardDate", DbType.String,AttendanceCheckInDtlForm.CardDate.ToString()),
                DataAccess.CreateParameter("CardTime", DbType.String,AttendanceCheckInDtlForm.CardTime.ToString()),
                DataAccess.CreateParameter("CheckInFlag", DbType.Int16,AttendanceCheckInDtlForm.CheckInFlag.ToString()),
                DataAccess.CreateParameter("CheckInEmployeeNo", DbType.String,AttendanceCheckInDtlForm.CheckInEmployeeNo),
                DataAccess.CreateParameter("CheckInDate", DbType.String,AttendanceCheckInDtlForm.CheckInDate.ToString()),
                DataAccess.CreateParameter("CheckInTime", DbType.String,AttendanceCheckInDtlForm.CheckInTime.ToString()),
                DataAccess.CreateParameter("CheckInEmployeeName", DbType.String,AttendanceCheckInDtlForm.CheckInEmployeeName.ToString()),
                DataAccess.CreateParameter("CardNo", DbType.String,""),
                DataAccess.CreateParameter("CheckInDescription", DbType.String,AttendanceCheckInDtlForm.CheckInDescription.ToString()),
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

        public List<AppAttendanceModel> GetAppAttendance(string EmpNo, DateTime SDATE, DateTime EDATE)
        {
            List<AppAttendanceModel> AppAttendanceList =new List<AppAttendanceModel>();
            DataTable dt = null;
            
            dt = DbAccess.ExecuteDataTable("SELECT * FROM gvEmployeeAll WHERE EmployeeNo = @EmpNo ",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo.ToString())
                }
            );

            EmpModel Emp = new EmpModel();

            Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
            Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
            Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();

            for (; SDATE <= EDATE; SDATE=SDATE.AddDays(1))
            {
                AppAttendanceModel Obj = new AppAttendanceModel();
                Obj.EmployeeNo = Emp.EmployeeNo;
                Obj.EmployeeName = Emp.EmployeeName;
                Obj.EmployeeEName = Emp.EmployeeEName;
                Obj.CardDate = SDATE.ToString("yyyy-MM-dd") ;
                //Obj.EmployeeEName = Emp.EmployeeEName;

                string SQL = " SELECT * FROM (SELECT TOP 1 CardType,SUBSTRING ( CardDate, 1 ,4 )+ '-'+ SUBSTRING (CardDate , 5, 2 )+'-' + SUBSTRING( CardDate ,7 , 2) AS CardDate, SUBSTRING (CardTime , 1, 2 )+':' + SUBSTRING( CardTime ,3 , 2) as CardTime FROM tbAttendance WHERE EmployeeNo=@EmployeeNo AND CardType='1' AND CardDate=@CardDate  ORDER BY CardTime ASC ) AAA"
                          + " UNION ALL  "
                          + " SELECT * FROM (SELECT TOP 1 CardType,SUBSTRING ( CardDate, 1 ,4 )+ '-'+ SUBSTRING (CardDate , 5, 2 )+'-' + SUBSTRING( CardDate ,7 , 2) AS CardDate, SUBSTRING (CardTime , 1, 2 )+':' + SUBSTRING( CardTime ,3 , 2) as CardTime FROM tbAttendance WHERE EmployeeNo=@EmployeeNo AND CardType='2' AND CardDate=@CardDate  ORDER BY CardTime DESC ) BBB"
                          + " UNION ALL  "
                          + " SELECT * FROM (SELECT TOP 1 CardType,SUBSTRING ( CardDate, 1 ,4 )+ '-'+ SUBSTRING (CardDate , 5, 2 )+'-' + SUBSTRING( CardDate ,7 , 2) AS CardDate, SUBSTRING (CardTime , 1, 2 )+':' + SUBSTRING( CardTime ,3 , 2) as CardTime FROM tbAttendance WHERE EmployeeNo=@EmployeeNo AND CardType='3' AND CardDate=@CardDate  ORDER BY CardTime ASC ) CCC"
                          + " UNION ALL  "
                          + " SELECT * FROM (SELECT TOP 1 CardType,SUBSTRING ( CardDate, 1 ,4 )+ '-'+ SUBSTRING (CardDate , 5, 2 )+'-' + SUBSTRING( CardDate ,7 , 2) AS CardDate, SUBSTRING (CardTime , 1, 2 )+':' + SUBSTRING( CardTime ,3 , 2) as CardTime FROM tbAttendance WHERE EmployeeNo=@EmployeeNo AND CardType='4' AND CardDate=@CardDate  ORDER BY CardTime DESC ) DDD";

                dt = DbAccess.ExecuteDataTable(SQL,
                    new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, EmpNo.ToString()),
                        DataAccess.CreateParameter("CardDate", DbType.String, SDATE.ToString("yyyyMMdd").Trim()),
                    }
                );
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Obj.CardTimeList.Add(new AppAttendanceCardTime()
                    {
                        CardTime = dt.Rows[i]["CardTime"].ToString(),
                        CardType = dt.Rows[i]["CardType"].ToString(),
                    });
                }

                //若是等於0就不寫入
                if (Obj.CardTimeList.Count != 0)
                {
                    AppAttendanceList.Add(Obj);
                }
            }
           return AppAttendanceList;
        }
    }

}