using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHRModel;
using RegalHR.ModelFactory;
using Newtonsoft.Json;


namespace RegalHR.Controllers
{
    public class OutgoingController : BaseController
    {

        OutgoingModelFactory ModelFactory;
        string UserFreeStyle_id = "OutgoingGridViewer";

        public OutgoingController()
        {
            if (ModelFactory == null)
            {
               ModelFactory = new OutgoingModelFactory();
            }
        }




        public ActionResult ManageOutMan()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;



            string ProgramID = "W004";
            ProgramModel SysProgram = null;
            if (!GeneralObj.CheckProgID(LoginUserInfo.ProgramList, ProgramID))
            {
                return View("../SysMsg/NoAccess");
            }
            else
            {
                SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
                ViewData["SysProgram"] = SysProgram;
            }

            UserFreeStyleModel UserFreeStyle;
            UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());



            EmpModel Emp = ModelFactory.GetOutMan(UserFreeStyle.SignId);


            ViewData["Emp"] = Emp;


            return View();
        }





        public ActionResult Manage()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "W004";
            ProgramModel SysProgram = null;
            if (!GeneralObj.CheckProgID(LoginUserInfo.ProgramList, ProgramID))
            {
                return View("../SysMsg/NoAccess");
            }
            else
            {
                SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
                ViewData["SysProgram"] = SysProgram;
            }



            
            UserFreeStyleModel UserFreeStyle;

            try
            {
                if (Request["action"] == "BACK" && Session[UserFreeStyle_id] != null)
                {

                    UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());
                }
                else
                {
                    UserFreeStyle = new UserFreeStyleModel()
                    {
                        Name = UserFreeStyle_id,
                        OrderField = "EmployeeNo",
                        OrderType = "ASC",
                        PageNum = "0",
                        SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                        SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                        SearchEmpNo = "",//搜尋人員
                        SearchEmpStatus = "1", //搜尋 離職狀態
                        SearchText = "", //搜尋狀態
                    };
                    
                }
            }
            catch
            {
                UserFreeStyle = new UserFreeStyleModel()
                {
                    Name = UserFreeStyle_id,
                    OrderField = "EmployeeNo",
                    OrderType = "ASC",
                    PageNum = "0",
                    SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                    SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                    SearchEmpNo = "",//搜尋人員
                    SearchEmpStatus = "1", //搜尋 離職狀態
                    SearchText = "", //搜尋狀態
                };

            }


            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;



            return View();

        }




        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "W003";
            ProgramModel SysProgram = null;
            if (!GeneralObj.CheckProgID(LoginUserInfo.ProgramList, ProgramID))
            {
                return View("../SysMsg/NoAccess");
            }
            else
            {
                SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
                ViewData["SysProgram"] = SysProgram;
            }


            return View();
        }



        /// <summary>
        /// 取得清單
        /// </summary>
        /// <param name="SDATE">起始日期</param>
        /// <param name="EDATE">結束日期</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEventList(string SDATE,string EDATE)
        {
            
            List<OutgoingEventModel> M = ModelFactory.GetEventList(LoginUserInfo.UserId, SDATE, EDATE);




             



            //將工作日寫入日歷中
            MainModelFactory MainFactory = new MainModelFactory();
            List<CalendarEventModel> HolidayList = new List<CalendarEventModel>();
            HolidayList.AddRange(MainFactory.GetHolidayList(LoginUserInfo.Company, SDATE, EDATE));
            for (int i = 0; i < HolidayList.Count; i++)
            {
                OutgoingEventModel Cal = new OutgoingEventModel();

                Cal.OutId = "";
                Cal.id = HolidayList[i].id;
                Cal.title = HolidayList[i].title;
                Cal.start = HolidayList[i].start;
                Cal.allDay = true;
                Cal.self = true;
                Cal.backgroundColor = HolidayList[i].backgroundColor;
                Cal.color = HolidayList[i].color;
                Cal.textColor = HolidayList[i].textColor;
                M.Add(Cal);
            } 
            



            return Json(M);
        }



        [HttpPost]
        public JsonResult GetOutManEventList(string Company,string OutMan, string SDATE, string EDATE)
        {

            List<OutgoingEventModel> M = ModelFactory.GetEventList(OutMan, SDATE, EDATE);





            //將工作日寫入日歷中
            MainModelFactory MainFactory = new MainModelFactory();
            List<CalendarEventModel> HolidayList = new List<CalendarEventModel>();
            HolidayList.AddRange(MainFactory.GetHolidayList(Company, SDATE, EDATE));
            for (int i = 0; i < HolidayList.Count; i++)
            {
                OutgoingEventModel Cal = new OutgoingEventModel();

                Cal.OutId = "";
                Cal.id = HolidayList[i].id;
                Cal.title = HolidayList[i].title;
                Cal.start = HolidayList[i].start;
                Cal.allDay = true;
                Cal.self = true;
                Cal.backgroundColor = HolidayList[i].backgroundColor;
                Cal.color = HolidayList[i].color;
                Cal.textColor = HolidayList[i].textColor;
                M.Add(Cal);
            } 
            




            return Json(M);
        }


        

        /// <summary>
        /// 修改個人外出出勤紀錄
        /// </summary>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditOutgoing(string OutgoingJson)
        {
            OutgoingEventModel Outgoing = null;
            try
            {


                Outgoing = JsonConvert.DeserializeObject<OutgoingEventModel>(OutgoingJson);

                //檢查預約日期合不合法 , 若不合法 報錯!
                CheckOutDatetime(Outgoing.OutDate, Outgoing.OutTime);
                CheckOutDatetime(Outgoing.OutDate, Outgoing.GoOutTime);

                Outgoing.OutMan = LoginUserInfo.UserId;
                Outgoing.RecordMan = LoginUserInfo.UserId;
                Outgoing.Company = LoginUserInfo.Company;
                Outgoing.Status = "U";//編輯
                Outgoing.OutDate = Outgoing.OutDate.Replace("-", "");
                Outgoing.OutTime = Outgoing.OutTime.Replace(":", "");
                Outgoing.GoOutTime = Outgoing.GoOutTime.Replace(":", "");

                Outgoing.OutDescription = "";





                if (ModelFactory.EditOutgoing(Outgoing))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("失敗!");//失敗
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }
        



        /// <summary>
        /// 刪除個人外出紀錄
        /// </summary>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteOutgoing(string OutgoingJson)
        {

            OutgoingEventModel Outgoing = null;
            try
            {
                Outgoing = JsonConvert.DeserializeObject<OutgoingEventModel>(OutgoingJson);
                Outgoing.OutMan = LoginUserInfo.UserId;
                Outgoing.RecordMan = LoginUserInfo.UserId;
                Outgoing.Status = "D";//刪除


                if (ModelFactory.DeleteOutgoing(Outgoing))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {
                    return Json("失敗!");//失敗
                }




            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }



        /// <summary>
        /// 新增外出個人紀錄
        /// </summary>
        /// <param name="OutgoingFormJson"></param>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertOutgoing(string OutgoingFormJson,string OutgoingJson)
        {

            List<OutgoingEventModel> OutgoingList = null;

            OutgoingFormModel OutgoingForm = null;
            try
            {
                OutgoingList = JsonConvert.DeserializeObject<List<OutgoingEventModel>>(OutgoingJson);
                OutgoingForm = JsonConvert.DeserializeObject<OutgoingFormModel>(OutgoingFormJson);


                //檢查預約日期合不合法 , 若不合法 報錯!
                for (int i = 0; i < OutgoingList.Count; i++)
                {

                    CheckOutDatetime(OutgoingForm.SDate, OutgoingList[i].OutTime);//驗證時間是否
                    OutgoingList[i].OutTime = OutgoingList[i].OutTime.Replace(":", "");



                    CheckOutDatetime(OutgoingForm.SDate, OutgoingList[i].GoOutTime);//驗證時間是否
                    OutgoingList[i].GoOutTime = OutgoingList[i].GoOutTime.Replace(":", "");
                }


                OutgoingForm.OutMan = LoginUserInfo.UserId;
                OutgoingForm.RecordMan = LoginUserInfo.UserId;
                OutgoingForm.OutManCompany = LoginUserInfo.Company;
                OutgoingForm.SDate =OutgoingForm.SDate.Replace("-","");
                OutgoingForm.OutDescription = "";





      
                if (ModelFactory.InsertOutgoing(OutgoingForm,OutgoingList,"A"))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("失敗!");//失敗
                }




            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }







       public static void CheckOutDatetime(string OutDate, string OutTime, bool Manage=false)
       {
           try
           {

               DateTime SDATE;


               if (OutDate.Trim() == "" || OutTime.Trim() == "" || OutTime.Length != 5 || !DateTime.TryParse(OutDate + " " + OutTime, out SDATE))
               {
                   throw new Exception("日期格式不合法!");
               }

               
               DateTime Today =  DateTime.Parse(DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"));


               /*停用作廢
               if ( !Manage && SDATE.Ticks < Today.Ticks)
               {
                   throw new Exception("日期時間不得小於" + Today.ToString("yyyy-MM-dd HH:mm"));
               }
               */

               if ( !Manage && SDATE.Ticks >= Today.AddDays(90).Ticks)
               {
                   throw new Exception("日期不得超過90日!");
               }


           }catch(Exception ex){

               throw new Exception(ex.Message);
           }
           catch
           {
               throw new Exception("系統發生錯誤!");
           }

       }











        [HttpPost]
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../Outgoing/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            //初始化
            int RecordTotal = 0;
            List<EmpModel> DataList = new List<EmpModel>();


            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());

            string SQLWhere = "";
            SQLWhere += UserFreeStyle.SearchCompany != "" ? " AND Company = '" + UserFreeStyle.SearchCompany + "' " : "";

            SQLWhere += UserFreeStyle.SearchDepartMentNo != "" ? " AND DepartMentNo = '" + UserFreeStyle.SearchDepartMentNo + "' " : "";


            SQLWhere += UserFreeStyle.SearchEmpNo != "" ? " AND EmployeeNo = '" + UserFreeStyle.SearchEmpNo + "' " : "";


            SQLWhere += UserFreeStyle.SearchEmpStatus != "" ? " AND Status = '" + UserFreeStyle.SearchEmpStatus + "' " : "";


            SQLWhere += UserFreeStyle.SearchText != "" ? " AND (EmployeeNo LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeEName LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeName LIKE '%" + UserFreeStyle.SearchText + "%' OR CardNo = '" + UserFreeStyle.SearchText + "' ) " : "";




            string SQLOrderby = UserFreeStyle.OrderField + " " + UserFreeStyle.OrderType;



            //賦予資料清單及資料總筆數
            ModelFactory.GetGridViewList(SQLWhere, SQLOrderby, PageNum, GeneralObj.GridViewerLimit, ref DataList, ref RecordTotal);

            GViewer.DataList.AddRange((DataList.ToList()));






            GViewer.Page = PageNum;
            GViewer.PageLimit = GeneralObj.GridViewerLimit;


            GViewer.RecordTotal = RecordTotal;
            GViewer.PageTotal = GViewer.GetPageTotal(RecordTotal, GeneralObj.GridViewerLimit);
            GViewer.TargetURL = TargetURL;

            return Json(GViewer);
        }



        [HttpPost]
        public JsonResult SaveUserFreeStyle(string UserFreeStyle)
        {
            Session[UserFreeStyle_id] = UserFreeStyle;

            return Json("1");//代表已經處裡完
        }


















        /// <summary>
        /// 修改個人外出出勤紀錄
        /// </summary>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditOutgoingManage(string OutMan,string OutgoingJson)
        {
            OutgoingEventModel Outgoing = null;
            try
            {

                Outgoing = JsonConvert.DeserializeObject<OutgoingEventModel>(OutgoingJson);

                //檢查預約日期合不合法 , 若不合法 報錯!
                CheckOutDatetime(Outgoing.OutDate, Outgoing.OutTime,true);
                CheckOutDatetime(Outgoing.OutDate, Outgoing.GoOutTime, true);

                EmpModel EMP = ModelFactory.GetOutMan(OutMan);

                Outgoing.OutMan = EMP.EmployeeNo;
                Outgoing.Company = EMP.Company;
                Outgoing.RecordMan = LoginUserInfo.UserId;
                Outgoing.Status = "UM";//編輯(補登專用)
                Outgoing.OutDate = Outgoing.OutDate.Replace("-", "");
                Outgoing.OutTime = Outgoing.OutTime.Replace(":", "");
                Outgoing.GoOutTime = Outgoing.GoOutTime.Replace(":", "");





                if (ModelFactory.EditOutgoing(Outgoing))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("失敗!");//失敗
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }




        /// <summary>
        /// 刪除個人外出紀錄
        /// </summary>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteOutgoingManage(string OutMan,string OutgoingJson)
        {

            OutgoingEventModel Outgoing = null;
            try
            {
                Outgoing = JsonConvert.DeserializeObject<OutgoingEventModel>(OutgoingJson);
                Outgoing.OutMan = OutMan;
                Outgoing.RecordMan = LoginUserInfo.UserId;
                Outgoing.Status = "DM";//刪除(補登專用)


                if (ModelFactory.DeleteOutgoing(Outgoing))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {
                    return Json("失敗!");//失敗
                }




            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }



        /// <summary>
        /// 新增外出個人紀錄
        /// </summary>
        /// <param name="OutgoingFormJson"></param>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertOutgoingManage(string OutMan,string OutgoingFormJson, string OutgoingJson)
        {

            List<OutgoingEventModel> OutgoingList = null;

            OutgoingFormModel OutgoingForm = null;
            try
            {
                OutgoingList = JsonConvert.DeserializeObject<List<OutgoingEventModel>>(OutgoingJson);
                OutgoingForm = JsonConvert.DeserializeObject<OutgoingFormModel>(OutgoingFormJson);


                //CheckOutDatetime(OutgoingForm.SDate, OutgoingForm.STime,true);


                //檢查預約日期合不合法 , 若不合法 報錯!
                for (int i = 0; i < OutgoingList.Count; i++)
                {


                    CheckOutDatetime(OutgoingForm.SDate, OutgoingList[i].OutTime,true);//驗證時間是否
                    OutgoingList[i].OutTime = OutgoingList[i].OutTime.Replace(":", "");



                    CheckOutDatetime(OutgoingForm.SDate, OutgoingList[i].GoOutTime, true);//驗證時間是否
                    OutgoingList[i].GoOutTime = OutgoingList[i].GoOutTime.Replace(":", "");
                }



                EmpModel EMP = ModelFactory.GetOutMan(OutMan);


                OutgoingForm.OutMan = EMP.EmployeeNo;
                OutgoingForm.OutManCompany = EMP.Company;
                OutgoingForm.RecordMan = LoginUserInfo.UserId;
                OutgoingForm.SDate = OutgoingForm.SDate.Replace("-", "");
                //OutgoingForm.STime = OutgoingForm.STime.Replace(":", "");
                

                




                if (ModelFactory.InsertOutgoing(OutgoingForm, OutgoingList,"AM"))
                {
                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("失敗!");//失敗
                }




            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }

        }






        public ActionResult History()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;





            string ProgramID = "W006";
            ProgramModel SysProgram = null;
            if (!GeneralObj.CheckProgID(LoginUserInfo.ProgramList, ProgramID))
            {
                //特例: 允許進入
                //SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
                SysProgram = new ProgramModel();
                SysProgram.ProgID = "W006";
                SysProgram.ProgName = "外出歷程記錄";
                SysProgram.ViewLevel = "";

                ViewData["SysProgram"] = SysProgram;
                //return View("../SysMsg/NoAccess");
            }
            else
            {
                SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
                ViewData["SysProgram"] = SysProgram;
            }





            UserFreeStyleModel UserFreeStyle;

            try
            {
                if (Request["action"] == "BACK" && Session[UserFreeStyle_id] != null)
                {

                    UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());
                }
                else
                {
                    UserFreeStyle = new UserFreeStyleModel()
                    {
                        Name = UserFreeStyle_id,
                        OrderField = "EmployeeNo",
                        OrderType = "ASC",
                        PageNum = "0",
                        SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                        SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                        SearchEmpNo = "",//搜尋人員
                        SearchEmpStatus = "1", //搜尋 離職狀態
                        SearchText = "", //搜尋狀態
                    };

                }
            }
            catch
            {
                UserFreeStyle = new UserFreeStyleModel()
                {
                    Name = UserFreeStyle_id,
                    OrderField = "",
                    OrderType = "ASC",
                    PageNum = "0",
                    SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                    SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                    SearchEmpNo = "",//搜尋人員
                    SearchEmpStatus = "1", //搜尋 離職狀態
                    SearchText = "", //搜尋狀態
                };

            }


            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;


            return View();
        }







        [HttpPost]
        public JsonResult GetOutHistory(string OutId, string OutMan)
        {
            List<OutgoingEventModel> DataList = new List<OutgoingEventModel>();

            //賦予資料清單及資料總筆數
            DataList = ModelFactory.GetOutHistory(OutId, OutMan);

            return Json(DataList);
        }



        
        [HttpPost]
        public JsonResult HistoryGridViewer(string ViewLevel, string SDATE, string EDATE, int PageNum)
        {


            string TargetURL = "../Outgoing/HistoryViewList";
            GridViewerModel GViewer = new GridViewerModel();

            //初始化
            int RecordTotal = 0;
            List<OutgoingEventModel> DataList = new List<OutgoingEventModel>();


            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());

            string SQLWhere = "";

            if (ViewLevel == "A" || ViewLevel == "B" || ViewLevel == "C")
            {
                SQLWhere += UserFreeStyle.SearchCompany != "" ? " AND Company = '" + UserFreeStyle.SearchCompany + "' " : "";

                SQLWhere += UserFreeStyle.SearchDepartMentNo != "" ? " AND DepartMentNo = '" + UserFreeStyle.SearchDepartMentNo + "' " : "";

                SQLWhere += UserFreeStyle.SearchEmpNo != "" ? " AND OutMan = '" + UserFreeStyle.SearchEmpNo + "'  " : "";
            }else{

                SQLWhere += " AND OutMan = '" + LoginUserInfo.UserId + "'  ";
            }

            

            DateTime cSDATE = DateTime.Parse(SDATE);
            DateTime cEDATE = DateTime.Parse(EDATE);


            SQLWhere += " AND OutDate BETWEEN '" + cSDATE.ToString("yyyyMMdd") + "' AND '" + cEDATE.ToString("yyyyMMdd") + "' ";


            string SQLOrderby = " OutDate,OutTime  ";



            //賦予資料清單及資料總筆數
            ModelFactory.GetHistoryViewList(SQLWhere, SQLOrderby, PageNum, GeneralObj.GridViewerLimit, ref DataList, ref RecordTotal);

            GViewer.DataList.AddRange((DataList.ToList()));






            GViewer.Page = PageNum;
            GViewer.PageLimit = GeneralObj.GridViewerLimit;


            GViewer.RecordTotal = RecordTotal;
            GViewer.PageTotal = GViewer.GetPageTotal(RecordTotal, GeneralObj.GridViewerLimit);
            GViewer.TargetURL = TargetURL;

            return Json(GViewer);
        }



    }
}
