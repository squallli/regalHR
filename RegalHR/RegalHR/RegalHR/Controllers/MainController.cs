using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Common;
using REGAL.Data.DataAccess;
using RegalHR.ModelFactory;
using RegalHRModel;
using Newtonsoft.Json;
namespace RegalHR.Controllers
{
    public class MainController : BaseController
    {
        

        MainModelFactory ModelFactory = new MainModelFactory();


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "X001";
            ProgramModel SysProgram = null;
            SysProgram = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
            ViewData["SysProgram"] = SysProgram;



            return View();
        }


        [HttpPost]
        public JsonResult GetCalendarAllList(string Company, string DepartMentNo,string EmpNo, string SDATE, string EDATE)
        {
            List<CalendarAllEventModel> M = ModelFactory.GetCalendarAllList(LoginUserInfo.UserId, Company, DepartMentNo, EmpNo, SDATE, EDATE);


            return Json(M);
        }




        [HttpPost]
        public JsonResult GetEventList(string Company, string DepartMentNo,string EmpNo, string SDATE)
        {
            SDATE = SDATE.Replace("-", "");
            List<CalendarAllEventModel> M = ModelFactory.GetDayEventList(LoginUserInfo.UserId, Company, DepartMentNo, EmpNo, SDATE, SDATE);

            return Json(M);
        }



        [HttpPost]
        public JsonResult GetEventListForTime(string Company, string DepartMentNo,string EmpNo, string SDATE)
        {
            SDATE = SDATE.Replace("-", "");
            List<CalendarEventModel> M = ModelFactory.GetDayEventListForTime(LoginUserInfo.UserId, Company, DepartMentNo, EmpNo, SDATE, SDATE);

            return Json(M);
        }






 



    }
}
