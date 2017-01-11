using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHRModel;
using RegalHR.ModelFactory;
using Newtonsoft.Json;
using System.Configuration;

namespace RegalHR.Controllers
{
    public class AppController : Controller
    {
        //
        // GET: /App/


        /// <summary>
        /// 外出紀錄
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="SDATE"></param>
        /// <param name="EDATE"></param>
        /// <returns></returns>
        public JsonResult Outgoing(string EmpNo, string SDATE, string EDATE)
        {
            OutgoingModelFactory ModelFactory = new OutgoingModelFactory();
            List<OutgoingEventModel> M = ModelFactory.GetEventList(EmpNo, SDATE, EDATE);
            return Json(M, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 出勤紀錄
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="SDATE"></param>
        /// <returns></returns>
        public JsonResult Attendance(string EmpNo, string SDATE, string EDATE)
        {
            List<AppAttendanceModel> AppAttendanceList = null;

            AttendanceModelFactory ModelFactory = new AttendanceModelFactory();

            SDATE = SDATE.Substring(0, 4) + "-" + SDATE.Substring(4, 2) + "-" + SDATE.Substring(6, 2);
            EDATE = EDATE.Substring(0, 4) + "-" + EDATE.Substring(4, 2) + "-" + EDATE.Substring(6, 2);

            DateTime cSDATE = DateTime.Parse(SDATE);
            DateTime cEDATE = DateTime.Parse(EDATE);
            AppAttendanceList = ModelFactory.GetAppAttendance(EmpNo, cSDATE, cEDATE);

            return Json(AppAttendanceList, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="LoginPwd"></param>
        /// <returns></returns>
        public JsonResult Login(string LoginId, string LoginPwd)
        {
            try
            {

                LoginModelFactory ModelFactory = new LoginModelFactory();
                UserModel LoginUserInfo = ModelFactory.Login(LoginId, LoginPwd);

                AppLoginModel Login = new AppLoginModel();
                if (LoginUserInfo == null)
                {
                    Login.Result = "0";
                    Login.UserId = "";
                    Login.UserName = "";
                    Login.UserEName = "";
                    Login.Company = "";
                    Login.CompanyName = "";
                }
                else
                {
                    Login.Result = "1";
                    Login.UserId = LoginUserInfo.UserId;
                    Login.UserName = LoginUserInfo.UserName;
                    Login.UserEName = LoginUserInfo.UserEName;
                    Login.Company = LoginUserInfo.Company;
                    Login.CompanyName = LoginUserInfo.CompanyName;
                }


                return Json(Login, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                AppLoginModel Login = new AppLoginModel();

                Login.Result = "0";
                Login.UserId = "";
                Login.UserName = "";
                Login.UserEName = "";
                Login.Company = "";
                Login.CompanyName = "";
                return Json(Login, JsonRequestBehavior.AllowGet);

            }
        }



        /// <summary>
        /// App取得登入資料
        /// </summary>
        /// <param name="AppOutgoing">JSON物件</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTempLogin(string EmpNo)
        {
            RegalBonModelFactory ModelFactory = new RegalBonModelFactory();

            JsonresultModel ResultModel = new JsonresultModel();
            ResultModel.Result = "0";


            try
            {
                TempLoginModel TempLogin = ModelFactory.GetTempLogin(EmpNo, true);

                //**計算COUNTDOWN
                try
                {
                    DateTime EndTime = DateTime.Parse(TempLogin.EndTime);
                    TimeSpan ts = EndTime - DateTime.Now;
                    int CountDown = ts.Seconds;
                    if (CountDown <= 0)
                    {
                        CountDown = 0;
                    }
                    TempLogin.CountDown = CountDown;
                }catch{

                }

                ResultModel.Query = TempLogin;
                ResultModel.Result = "1";
                
            }
            catch (Exception ex)
            {
                ResultModel.ErrorMsg = ex.Message;
            }
            catch
            {
                ResultModel.ErrorMsg = "系統發生錯誤";
            }



            return Json(ResultModel, JsonRequestBehavior.AllowGet);
        }














        /// <summary>
        /// App刪除個人資料
        /// </summary>
        /// <param name="AppOutgoing">JSON物件</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DeleteOutgoing(string OutId,string EmpNo)
        {

            OutgoingEventModel Outgoing = null;

            JsonresultModel ResultModel = new JsonresultModel();
            

            try
            {
                Outgoing = new OutgoingEventModel();
                Outgoing.OutId = OutId;
                Outgoing.OutMan = EmpNo;
                Outgoing.RecordMan = EmpNo;
                Outgoing.Status = "D";//刪除
                Outgoing.Equipment = "1"; //App


                OutgoingModelFactory ModelFactory = new OutgoingModelFactory();

                if (ModelFactory.DeleteOutgoing(Outgoing))
                {
                    ResultModel.Result = "1";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);//代表已經處裡完
                }
                else
                {
                    ResultModel.Result = "0";
                    ResultModel.ErrorMsg = "無法刪除外出記錄";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);//失敗
                }




            }
            catch (Exception ex)
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = ex.Message;
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = "系統發生錯誤";
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }

        }




        /// <summary>
        /// App新增外出個人紀錄
        /// </summary>
        /// <param name="AppOutgoing">JSON物件</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertOutgoing(AppOutgoingModel AppOutgoing)
        {

            JsonresultModel ResultModel = new JsonresultModel();
            try
            {


                //檢查時間格式 是否正確
                CheckOutDatetime(AppOutgoing.SDate, AppOutgoing.STime);
                CheckOutDatetime(AppOutgoing.SDate, AppOutgoing.GoOutTime);




                OutgoingFormModel OutgoingForm = new OutgoingFormModel();
                List<OutgoingEventModel> OutgoingList = new List<OutgoingEventModel>();
                

                //************************************
                OutgoingForm.OutMan = AppOutgoing.OutMan;
                OutgoingForm.OutManCompany = AppOutgoing.OutManCompany;
                OutgoingForm.RecordMan = AppOutgoing.OutMan;//記錄人 等於 申請人
                OutgoingForm.SDate = AppOutgoing.SDate;
                OutgoingForm.STime = AppOutgoing.STime;
                OutgoingForm.OutDescription = "";
                


                OutgoingList.Add(new OutgoingEventModel()
                {
                    Location = System.Web.HttpUtility.UrlDecode(AppOutgoing.Location),
                    CustomerName = System.Web.HttpUtility.UrlDecode(AppOutgoing.CustomerName),
                    OutDescription = System.Web.HttpUtility.UrlDecode(AppOutgoing.OutDescription),
                    OutTime =  OutgoingForm.STime.Replace(":", ""),
                    GoOutTime = AppOutgoing.GoOutTime.Replace(":", ""),
                    Equipment = "1", //App
                });


                
                
                OutgoingForm.SDate = OutgoingForm.SDate.Replace("-", "");




                OutgoingModelFactory ModelFactory = new OutgoingModelFactory();


                if (ModelFactory.InsertOutgoing(OutgoingForm, OutgoingList, "A"))
                {
                    ResultModel.Result = "1";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);//代表已經處裡完
                }
                else
                {
                    ResultModel.Result = "0";
                    ResultModel.ErrorMsg = "新增外出失敗";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = ex.Message;
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = "系統發生錯誤";
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }

        }












        /// <summary>
        /// 修改個人外出出勤紀錄
        /// </summary>
        /// <param name="OutgoingJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditOutgoing(AppOutgoingModel AppOutgoing)
        {

            JsonresultModel ResultModel = new JsonresultModel();
            try
            {


                CheckOutDatetime(AppOutgoing.SDate, AppOutgoing.STime);
                CheckOutDatetime(AppOutgoing.SDate, AppOutgoing.GoOutTime);

                OutgoingEventModel Outgoing = new OutgoingEventModel();


                OutgoingModelFactory ModelFactory = new OutgoingModelFactory();

                EmpModel EMP = ModelFactory.GetOutMan(AppOutgoing.OutMan);

                Outgoing.OutId = AppOutgoing.OutId;
                Outgoing.OutMan = EMP.EmployeeNo;
                Outgoing.Company = EMP.Company;
                Outgoing.RecordMan = EMP.EmployeeNo;

                Outgoing.Location = System.Web.HttpUtility.UrlDecode(AppOutgoing.Location);
                Outgoing.CustomerName = System.Web.HttpUtility.UrlDecode(AppOutgoing.CustomerName);
                Outgoing.OutDescription = "";
                Outgoing.Status = "U";//編輯
                Outgoing.OutDate = AppOutgoing.SDate.Replace("-", "");
                Outgoing.OutTime = AppOutgoing.STime.Replace(":", "");
                Outgoing.GoOutTime = AppOutgoing.GoOutTime.Replace(":", "");
                Outgoing.Equipment = "1";


                
                


                






                if (ModelFactory.EditOutgoing(Outgoing))
                {
                    ResultModel.Result = "1";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);//代表已經處裡完
                }
                else
                {
                    ResultModel.Result = "0";
                    ResultModel.ErrorMsg = "編輯外出失敗";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);//失敗
                }


            }
            catch (Exception ex)
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = ex.Message;
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = "系統發生錯誤";
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }

        }














        /// <summary>
        /// 檢查外出進來的格式時間
        /// </summary>
        /// <param name="OutDate"></param>
        /// <param name="OutTime"></param>
        public static void CheckOutDatetime(string OutDate, string OutTime)
        {
            try
            {

                DateTime SDATE;


                if (OutDate.Trim() == "" || OutTime.Trim() == "" || OutTime.Length != 5 || !DateTime.TryParse(OutDate + " " + OutTime, out SDATE))
                {
                    throw new Exception("日期格式不合法!");
                }


                DateTime Today = DateTime.Parse(DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm"));




                if (SDATE.Ticks >= Today.AddDays(90).Ticks)
                {
                    throw new Exception("日期不得超過90日!");
                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            catch
            {
                throw new Exception("系統發生錯誤!");
            }

        }








        [HttpGet]
        public JsonResult GetCalendarAllList(string Company, string EmpNo, string SDATE, string EDATE)
        {

            OutgoingModelFactory ModelFactory =new OutgoingModelFactory();
            List<OutgoingEventModel> Box =null;

            if (EmpNo == null || EmpNo == "")
            {
                //未登入-預設台北行事曆
                Box = new List<OutgoingEventModel>();
                Company = "TPE";
            }
            else
            {

                Box = ModelFactory.GetEventList(EmpNo, SDATE, EDATE);
            }



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
                Box.Add(Cal);
            } 
            

            return Json(Box, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public JsonResult GetEmpInfo(string EmpNo)
        {

            JsonresultModel ResultModel = new JsonresultModel();


            EmpModelFactory Factory = new EmpModelFactory();
            EmpModel Emp = null;
            try
            {

                ResultModel.Result = "1";

                Emp = Factory.GetEmpData(EmpNo);

                ResultModel.Query = Emp;
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = ex.Message;
                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }


            
        }




         /// <summary>
        /// 取得App版本
        /// </summary>
        [HttpGet]
        public JsonResult GetAppVersion(string AppOS)
        {

            
            JsonresultModel ResultModel = new JsonresultModel();
            AppVersionModel AppVersion = new AppVersionModel();


            try
            {


                if (AppOS == "IOS")
                {
                    //iphone
                    AppVersion.AppOS = "IOS";
                    AppVersion.Version = ConfigurationManager.AppSettings["IOS_Version"];
                    AppVersion.Download = ConfigurationManager.AppSettings["IOS_Download"];
                    ResultModel.Result = "1";
                    ResultModel.Query = AppVersion;
                }
                else if (AppOS == "Android")
                {
                    //android
                    AppVersion.AppOS = "Android";



                    AppVersion.Version = GeneralModelFactory.getAPKVersion(Server.MapPath("/files/app.apk"));
                    AppVersion.Download = ConfigurationManager.AppSettings["Android_Download"];

                    
                    ResultModel.Result = "1";
                    ResultModel.Query = AppVersion;
                }
                else
                {
                    ResultModel.Result = "0";
                    ResultModel.ErrorMsg = "查無AppOS";
                    return Json(ResultModel, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                ResultModel.Result = "0";
                ResultModel.ErrorMsg = "系統發生錯誤";
                return Json(ResultModel, JsonRequestBehavior.AllowGet);

            }

            return Json(ResultModel, JsonRequestBehavior.AllowGet);
        }



       


        public ActionResult Download_IOS()
        {
            var path = Server.MapPath("/files/app.apk");
            var mime = "application/apk";
            var filename = "app.apk";

            return File(path, mime, filename);
        }

        public ActionResult Download_Android()
        {
            var path = Server.MapPath("/files/app.apk");
            var mime = "application/apk";
            var filename = "app.apk";
            return File(path, mime, filename);
        }
    }
}
