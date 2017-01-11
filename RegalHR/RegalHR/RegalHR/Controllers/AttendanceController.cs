using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Common;
using RegalHR.ModelFactory;
using RegalHRModel;
using Newtonsoft.Json;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.DDF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XSSF;

namespace RegalHR.Controllers
{
    public class AttendanceController : BaseController
    {

        AttendanceModelFactory ModelFactory = new AttendanceModelFactory();


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "W001";
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


        [HttpPost]
        public JsonResult GetEmpAllList(string DepartMentNo)
        {
            List<EmpModel> EmpList = null;

            EmpList = ModelFactory.GetEmpAllList(DepartMentNo);

            return Json(EmpList);

        }

        public ActionResult Manage()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "W002";
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
            UserFreeStyle = new UserFreeStyleModel()
            {
                Name = "AttendanceManage",
                OrderField = "",
                OrderType = "",
                PageNum = "",
                SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                SearchEmpNo = "",//搜尋人員
                SearchEmpStatus = "1", //搜尋 離職狀態
                SearchText = "", //搜尋狀態
            };

            ViewData["UserFreeStyle"] = UserFreeStyle;

            return View();
        }

        [HttpPost]
        public JsonResult GetAttendanceList(string SDATE, string EDATE, int Skip)
        {
            List<AttendanceModel> AttendanceList = null;

            try
            {
                DateTime cSDATE = DateTime.Parse(SDATE);
                DateTime cEDATE = DateTime.Parse(EDATE);
                AttendanceList = ModelFactory.GetAttendanceList(0, "", "", LoginUserInfo.UserId, "", cSDATE, cEDATE, "1", GeneralObj.AttendanceLimit, Skip);
                return Json(AttendanceList);
            }
            catch (Exception ex)
            {
                return Json("0|" + ex.Message);
            }
            catch
            {
                return Json("0|系統發生錯誤!");
            } 
        }

        [HttpPost]
        public JsonResult GetManageAttendanceList(string UserFreeStyleJson, string SDATE, string EDATE, int Skip)
        {
            List<AttendanceModel> AttendanceList = null;
            UserFreeStyleModel UserFreeStyle = null;
            try
            {

                UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(UserFreeStyleJson);

                
                DateTime cSDATE = DateTime.Parse(SDATE);
                DateTime cEDATE = DateTime.Parse(EDATE);
                AttendanceList = ModelFactory.GetAttendanceList(1, UserFreeStyle.SearchCompany, UserFreeStyle.SearchDepartMentNo, UserFreeStyle.SearchEmpNo, UserFreeStyle.SearchText, cSDATE, cEDATE, "1", GeneralObj.AttendanceLimit, Skip);
                return Json(AttendanceList);
            }
            catch (Exception ex)
            {
                return Json("0|" + ex.Message);
            }
            catch
            {
                return Json("0|系統發生錯誤!");
            }
            
        }


        /// <summary>
        /// 回傳補登表單
        /// </summary>
        /// <param name="SDATE"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCheckInDtlForm(string SDATE, string EmpNo)
        {

            AttendanceCheckInDtlModel AttendanceCheckInDtl = ModelFactory.GetAttendanceCheckInDtl(SDATE, EmpNo);

            return Json(AttendanceCheckInDtl);

        }

        /// <summary>
        /// 回傳個人查詢詳細資料
        /// </summary>
        /// <param name="SDATE"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetShowDtl(string SDATE)
        {

            AttendanceShowDtlModel AttendanceShowDtl = ModelFactory.GetAttendanceShowDtl(SDATE, LoginUserInfo.UserId);

            return Json(AttendanceShowDtl);

        }

        /// <summary>
        /// 回傳查詢詳細資料
        /// </summary>
        /// <param name="SDATE"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetShowDtlManage(string SDATE,string EmpNo)
        {
            AttendanceShowDtlModel AttendanceShowDtl = ModelFactory.GetAttendanceShowDtl(SDATE, EmpNo);

            return Json(AttendanceShowDtl);
        }

       //存檔
        [HttpPost]
        public JsonResult SaveCheckInDtlForm(string AttendanceCheckInDtlJson)
        {

            AttendanceCheckInDtlModel AttendanceCheckInDtl = null;

            try
            {

                AttendanceCheckInDtl = JsonConvert.DeserializeObject<AttendanceCheckInDtlModel>(AttendanceCheckInDtlJson);

                AttendanceCheckInDtl.CheckInFlag = "1";
                AttendanceCheckInDtl.CheckInEmployeeNo = LoginUserInfo.UserId;
                AttendanceCheckInDtl.CheckInEmployeeName = LoginUserInfo.UserName;
                
                AttendanceCheckInDtl.CheckInDate = DateTime.Now.ToString("yyyyMMdd");
                AttendanceCheckInDtl.CheckInTime = DateTime.Now.ToString("HHmmss");

                AttendanceCheckInDtl.CardDate = AttendanceCheckInDtl.CardDate.Replace("-","");
                AttendanceCheckInDtl.CardTime = AttendanceCheckInDtl.CardTime.Replace(":", "");

                if (ModelFactory.SaveCheckInDtlForm(AttendanceCheckInDtl))
                {
                    return Json("1");
                }
                else
                {
                    return Json("0|新增失敗!"); 
                }
  
            }
            catch (Exception ex)
            {
                return  Json("0|新增失敗!"); 
            }
        }

        //出勤補登介面
        public ActionResult AttendanceCheckIn()
        {

            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "W005";
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
            UserFreeStyle = new UserFreeStyleModel()
            {
                Name = "AttendanceCheckIn",
                OrderField = "",
                OrderType = "",
                PageNum = "",
                SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                SearchDepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram),
                SearchEmpNo = "",//搜尋人員
                SearchEmpStatus = "1", //搜尋 離職狀態
                SearchText = "", //搜尋狀態
            };

            ViewData["UserFreeStyle"] = UserFreeStyle;

            return View();
        }
    }
}
