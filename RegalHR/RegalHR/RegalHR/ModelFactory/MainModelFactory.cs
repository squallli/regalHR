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
    public class MainModelFactory
    {
        public DataAccess DbAccess = new DataAccess();

        public MainModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }





        public List<CalendarAllEventModel> GetCalendarAllList(string Own, string Company, string DepartMentNo, string EmpNo, string SDATE, string EDATE)
        {

            List<CalendarAllEventModel> query = new List<CalendarAllEventModel>();

            
            List<CalendarEventModel> CalenderList = new List<CalendarEventModel>();




            //將外出寫入日歷中
            CalenderList.AddRange(GetEventList(Own, Company, DepartMentNo, EmpNo, SDATE, EDATE));





            //群組組合 By 日期,人
            var tmpList = from tmp in CalenderList
                          group tmp by new { tmp.OwnFlag, tmp.start, tmp.EmployeeNo, tmp.EmployeeName, tmp.EmployeeEName } into g
                          select new { OwnFlag = g.Key.OwnFlag, EmployeeNo = g.Key.EmployeeNo, EmployeeName = g.Key.EmployeeName, EmployeeEName = g.Key.EmployeeEName, start = g.Key.start, List = g.ToList() };


            tmpList = (from tmp in tmpList
                       orderby tmp.OwnFlag descending
                       select tmp);

            int DayDisplayNum = 5;
            foreach (var tmp in tmpList)
            {

                int QueryNum = (from tmpQuery in query
                                where tmpQuery.start == tmp.start || tmpQuery.start == tmp.start + " 23:59" || tmpQuery.start == tmp.start + " 09:00"
                                select tmpQuery).Count();


                //當目前筆數第四筆時判斷是否要寫入第五筆
                if (QueryNum == DayDisplayNum - 1)
                {
                    int Num = (from tmpQuery in tmpList
                               where tmpQuery.start == tmp.start
                               select tmpQuery).Count();

                    if (Num > DayDisplayNum)
                    {
                        //當此日期數 大於 五筆 回寫還有多少筆
                        query.Add(new CalendarAllEventModel()
                        {
                            id = tmp.start + "@END",
                            title = "還有 " + (Num - (DayDisplayNum - 1)).ToString() + " 筆 ",
                            start = tmp.start + " 23:59",
                            allDay = true,
                            self = true,
                            backgroundColor = "transparent",
                            color = "transparent",
                            textColor = "#000",
                            className = "EventMore"
                        });

                        continue;
                    }


                }
                else if (QueryNum >= 5)
                {
                    //不允許寫入超過五筆
                    continue;
                }





                string color = "#E0F2F7";
                //string OwnTime = " 00:00";
                if (Own.Trim() == tmp.EmployeeNo)
                {
                    color = "#F7D358";//登入者的顏色
                }

                CalendarAllEventModel Cal = new CalendarAllEventModel();

                Cal.EmployeeNo = tmp.EmployeeNo;
                Cal.EmployeeName = tmp.EmployeeName;
                Cal.EmployeeEName = tmp.EmployeeEName;
                Cal.id = tmp.start + "@" + tmp.EmployeeNo;
                Cal.title = tmp.EmployeeName + " - " + Cal.EmployeeEName ;

                if (tmp.start.Length>=11)
                {
                    Cal.start = tmp.start;
                }
                else
                {
                    Cal.start = tmp.start+" 09:00";
                }

                Cal.allDay = true;
                Cal.self = true;
                Cal.EventList = tmp.List;
                Cal.backgroundColor = color;
                query.Add(Cal);

            }








            //將工作日寫入日歷中
            List<CalendarEventModel> HolidayList = new List<CalendarEventModel>();
            HolidayList.AddRange(GetHolidayList(Company, SDATE, EDATE));
            for (int i = 0; i < HolidayList.Count; i++)
            {
                CalendarAllEventModel Cal = new CalendarAllEventModel();

                Cal.EmployeeNo = "";
                Cal.EmployeeName = "";
                Cal.EmployeeEName = "";
                Cal.id = HolidayList[i].id;
                Cal.title = HolidayList[i].title;
                Cal.start = HolidayList[i].start;
                Cal.allDay = true;
                Cal.self = true;
                Cal.backgroundColor = HolidayList[i].backgroundColor;
                Cal.color = HolidayList[i].color;
                Cal.textColor = HolidayList[i].textColor;
                query.Add(Cal);

            } 
            

            





            return query;
        }













        public List<CalendarEventModel> GetDayEventListForTime(string Own, string Company, string DepartMentNo, string EmpNo, string SDATE, string EDATE)
        {


            List<CalendarEventModel> OutList = GetEventListForTime(Own, Company, DepartMentNo, EmpNo, SDATE, EDATE);


            var tmpList = (from tmp in OutList
                                                orderby tmp.start ascending
                                                select tmp).ToList();
            return tmpList;
        }





        public List<CalendarAllEventModel> GetDayEventList(string Own, string Company, string DepartMentNo, string EmpNo, string SDATE, string EDATE)
        {
            List<CalendarAllEventModel> query = new List<CalendarAllEventModel>();

            List<CalendarEventModel> OutList = GetEventList(Own, Company, DepartMentNo, EmpNo, SDATE, EDATE);

            //群組組合 By 日期,人
            var tmpList = from tmp in OutList
                          group tmp by new { tmp.OwnFlag, tmp.start, tmp.EmployeeNo, tmp.EmployeeName, tmp.EmployeeEName } into g
                          select new { OwnFlag = g.Key.OwnFlag, EmployeeNo = g.Key.EmployeeNo, EmployeeName = g.Key.EmployeeName, EmployeeEName = g.Key.EmployeeEName, start = g.Key.start, List = g.ToList() };



            foreach (var tmp in tmpList)
            {

                CalendarAllEventModel Cal = new CalendarAllEventModel();
                Cal.EmployeeNo = tmp.EmployeeNo;
                Cal.EmployeeName = tmp.EmployeeName;
                Cal.EmployeeEName = tmp.EmployeeEName;
                Cal.id = tmp.start + "@" + tmp.EmployeeNo;
                Cal.title = tmp.EmployeeName;
                Cal.start = tmp.start;
                Cal.allDay = true;
                Cal.self = true;
                Cal.EventList = tmp.List;
                query.Add(Cal);

            }


            return query;
        }




        public  List<CalendarEventModel> GetHolidayList(string Company,string SDATE,string EDATE)
        {


            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvHolidayDtl WHERE Display='1' AND  Company=@Company AND Holiday BETWEEN @SDATE AND @EDATE ",
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString()),
                }
            );



            List<CalendarEventModel> HolidayList = new List<CalendarEventModel>();

            string Title = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Title = dt.Rows[i]["CtypeName"].ToString();

                if (dt.Rows[i]["Memo"].ToString().Trim() !="")
                {
                    Title += " ("+dt.Rows[i]["Memo"].ToString()+")";
                }

                string Color = "#ff0000";
                if (dt.Rows[i]["Ctype"].ToString()=="0")
                {
                    Color = "#0000ff";
                }

                HolidayList.Add(new CalendarEventModel()
                {
                    id = "Holiday"+dt.Rows[i]["Holiday"].ToString(),
                    title = Title,
                    start = dt.Rows[i]["start"].ToString()+" 00:00",
                    allDay = true,
                    self = true,
                    backgroundColor = "transparent",
                    color = "transparent",
                    textColor = Color,
                });
            }



            return HolidayList;
        }








        public List<CalendarEventModel> GetEventList(string Own, string Company, string DepartMentNo, string EmpNo, string SDATE, string EDATE)
        {

            string sql = "SELECT * FROM (SELECT CASE WHEN OutMan=@Own THEN '1' ELSE '0' END as OwnFlag,*,Convert(char(10),Convert(datetime,OutDate),20) as start,left(OutTime,2)+':'+right(OutTime,2) as StartTime FROM gvOutgoing WHERE Status IN ('A','U','AM','UM')  AND OutDate BETWEEN @SDATE AND @EDATE ";

            //公司條件
            if (Company!="")
            {
                sql += " AND Company = @Company ";
            }


            //部門條件
            if (DepartMentNo != "")
            {
                sql += " AND DepartMentNo = @DepartMentNo ";
            }


            //人員條件
            if (EmpNo != "")
            {
                sql += " AND OutMan = @OutMan ";
            }

            sql += ") AAA ";

            DataTable dt = DbAccess.ExecuteDataTable(sql,
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, DepartMentNo.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, EmpNo.ToString()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString()),
                    DataAccess.CreateParameter("Own", DbType.String, Own.ToString()),
                }
            );
            List<CalendarEventModel> OutList = new List<CalendarEventModel>();




            for (int i = 0; i < dt.Rows.Count; i++)
            {
                OutList.Add(new CalendarEventModel()
                {

                    RecordMan = dt.Rows[i]["RecordManName"].ToString(),
                    UpdateDate = dt.Rows[i]["UpdateDate"].ToString(),
                    UpdateTime = dt.Rows[i]["UpdateTime"].ToString(),
                    id = dt.Rows[i]["OutId"].ToString()+dt.Rows[i]["OutMan"].ToString(),
                    EmployeeNo = dt.Rows[i]["OutMan"].ToString().Trim(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString().Trim(),
                    EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString().Trim(),
                    title = dt.Rows[i]["startTime"].ToString() + " 地點:" + dt.Rows[i]["Location"].ToString() + " , 客戶:" + dt.Rows[i]["CustomerName"].ToString(),
                    start = dt.Rows[i]["start"].ToString(),
                    OwnFlag = dt.Rows[i]["OwnFlag"].ToString(),
                    allDay = true,
                    self = true,
                });
            }


            return OutList;
        }












        public List<CalendarEventModel> GetEventListForTime(string Own, string Company, string DepartMentNo, string EmpNo, string SDATE, string EDATE)
        {
            string sql = "SELECT * FROM (SELECT CASE WHEN OutMan=@Own THEN '1' ELSE '0' END as OwnFlag, *,Convert(char(10),Convert(datetime,OutDate),20) as start,left(OutTime,2)+':'+right(OutTime,2) as StartTime, ";

            sql += "Convert(char(10),Convert(datetime,UpdateDate),20) +' '+ left(UpdateTime,2)+':'+right(left(UpdateTime,4),2)  AS UpdateDateTime, left(GoOutTime,2)+':'+right(left(GoOutTime,4),2)  AS GoOutTimeFormat ";
            sql += " FROM gvOutgoing WHERE Status IN ('A','U','AM','UM')  AND OutDate BETWEEN @SDATE AND @EDATE ";



            //公司條件
            if (Company != "")
            {
                sql += " AND Company = @Company ";
            }


            //部門條件
            if (DepartMentNo != "")
            {
                sql += " AND DepartMentNo = @DepartMentNo ";
            }


            //人員條件
            if (EmpNo != "")
            {
                sql += " AND OutMan = @OutMan ";
            }

            sql += ") AAA ";

            DataTable dt = DbAccess.ExecuteDataTable(sql,
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, Company.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, DepartMentNo.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, EmpNo.ToString()),
                    DataAccess.CreateParameter("SDATE", DbType.String, SDATE.ToString()),
                    DataAccess.CreateParameter("EDATE", DbType.String, EDATE.ToString()),
                    DataAccess.CreateParameter("Own", DbType.String, Own.ToString()),
                }
            );
            List<CalendarEventModel> OutList = new List<CalendarEventModel>();




            for (int i = 0; i < dt.Rows.Count; i++)
            {
                OutList.Add(new CalendarEventModel()
                {

                    RecordMan = dt.Rows[i]["RecordManName"].ToString(),
                    UpdateTime = dt.Rows[i]["UpdateDateTime"].ToString(),
                    id = dt.Rows[i]["OutId"].ToString() + dt.Rows[i]["OutMan"].ToString(),
                    EmployeeNo = dt.Rows[i]["OutMan"].ToString().Trim(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString().Trim(),
                    EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString().Trim(),
                    title = " 地點:" + dt.Rows[i]["Location"].ToString() + " , 客戶:" + dt.Rows[i]["CustomerName"].ToString(),
                    start = dt.Rows[i]["startTime"].ToString(),
                    OwnFlag = dt.Rows[i]["OwnFlag"].ToString(),
                    Status = dt.Rows[i]["Status"].ToString(),
                    GoOutTime = dt.Rows[i]["GoOutTimeFormat"].ToString(),
                    allDay = true,
                    self = true,
                });
            }


            return OutList;
        }


    }
}