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
    public class ReportAttendanceController : BaseController
    {
        ReportAttendanceModelFactory ModelFactory = new ReportAttendanceModelFactory();

        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;
            return View();
        }

        /// <summary>
        /// 取得該作業可用的報表清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetReportList()
        {
            JsonresultModel ResultModel = new JsonresultModel();
            ResultModel.Result = "0";

            try
            {
                List<ReportModel> ReportList = ModelFactory.GetReportList(LoginUserInfo.ProgramList);
                ResultModel.Result = "1";
                ResultModel.Query = ReportList;
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
        /// RA01 出勤記錄報表
        /// </summary>
        /// <returns></returns>
        public ActionResult RA01()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "RA01";
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

            ReportFormModel ReportForm=new ReportFormModel();
            ReportForm.Company = LoginUserInfo.Company;
            ReportForm.DepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram);
            ReportForm.EmpNo = "";
            ReportForm.StartDate = DateTime.Now.ToString("yyyy-MM-dd");
            ReportForm.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            
            ReportForm.EmpStatus = "1";

            ViewData["ReportForm"] = ReportForm;

            return View();
        }


        [HttpPost]
        public JsonResult GetRA01ToExcel(ReportFormModel ReportForm)
        {
           //初始****
           if (ReportForm.EmpNo == null)ReportForm.EmpNo = "";
           if (ReportForm.Company == null) ReportForm.Company = "";
           if (ReportForm.DepartMentNo == null) ReportForm.DepartMentNo = "";
           if (ReportForm.EndDate == null) ReportForm.EndDate = "";
           if (ReportForm.StartDate == null) ReportForm.StartDate = "";
           if (ReportForm.EmpStatus == null) ReportForm.EmpStatus = "";

           JsonresultModel ResultModel = new JsonresultModel();
           ResultModel.Result = "0";

           try
            {
                DataTable AttendanceDT = null;

                DateTime StartDate = DateTime.Parse(ReportForm.StartDate);
                DateTime EndDate = DateTime.Parse(ReportForm.EndDate);

               //取得出勤資料用
                AttendanceModelFactory AttendanceModelFactory = new AttendanceModelFactory();
                AttendanceDT = AttendanceModelFactory.GetAttendanceDataTable(2, ReportForm.Company, ReportForm.DepartMentNo, ReportForm.EmpNo, "", StartDate, EndDate, ReportForm.EmpStatus, 100000, 0);

                string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

                //SavePath=儲存路徑
                string filenameStr = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Download\\" + filename;

                using (MemoryStream msExcel = ReportAttendanceModelFactory.RA01ToExcel(AttendanceDT) as MemoryStream)// 新增試算表
                {
                    byte[] data = msExcel.ToArray();
                    using (FileStream fileStream = new FileStream(filenameStr, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
  
                //賦予報表位址
                ExcelModel Excel =new ExcelModel();
                Excel.Url="../Download/" + filename;
                ResultModel.Result = "1";
                ResultModel.Query = Excel;

                return Json(ResultModel, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                return Json(ResultModel, JsonRequestBehavior.AllowGet); 
            }
        }

        /// <summary>
        /// RA02 員工出勤補登記錄表
        /// </summary>
        /// <returns></returns>
        public ActionResult RA02()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "RA02";
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

            ReportFormModel ReportForm = new ReportFormModel();
            ReportForm.Company = LoginUserInfo.Company;
            ReportForm.DepartMentNo = GeneralObj.SearchDepartMentDefault(LoginUserInfo, SysProgram);
            ReportForm.EmpNo = "";
            ReportForm.StartDate = DateTime.Now.ToString("yyyy-MM-dd");
            ReportForm.EndDate = DateTime.Now.ToString("yyyy-MM-dd");
            ReportForm.EmpStatus = "1";

            ViewData["ReportForm"] = ReportForm;

            return View();
        }


        [HttpPost]
        public JsonResult GetRA02ToExcel(ReportFormModel ReportForm)
        {
            //初始****
            if (ReportForm.EmpNo == null) ReportForm.EmpNo = "";
            if (ReportForm.Company == null) ReportForm.Company = "";
            if (ReportForm.DepartMentNo == null) ReportForm.DepartMentNo = "";
            if (ReportForm.EndDate == null) ReportForm.EndDate = "";
            if (ReportForm.StartDate == null) ReportForm.StartDate = "";
            if (ReportForm.EmpStatus == null) ReportForm.EmpStatus = "";

            JsonresultModel ResultModel = new JsonresultModel();
            ResultModel.Result = "0";

            try
            {
                DataTable AttendanceDT = null;

                // DateTime StartDate = DateTime.Parse(ReportForm.StartDate);
                // DateTime EndDate = DateTime.Parse(ReportForm.EndDate);

                string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";

                //SavePath=儲存路徑
                string filenameStr = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Download\\" + filename;

                using (MemoryStream msExcel = ModelFactory.RA02ToExcel(ReportForm) as MemoryStream)// 新增試算表
                {
                    byte[] data = msExcel.ToArray();
                    using (FileStream fileStream = new FileStream(filenameStr, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }

                //賦予報表位址
                ExcelModel Excel = new ExcelModel();
                Excel.Url = "../Download/" + filename;
                ResultModel.Result = "1";
                ResultModel.Query = Excel;

                return Json(ResultModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ResultModel, JsonRequestBehavior.AllowGet);

            }
        }
    }
}
