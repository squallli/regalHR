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
    public class ReportOutgoingController : BaseController
    {


        ReportOutgoingModelFactory ModelFactory = new ReportOutgoingModelFactory();

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
        /// RG01 外出異常表
        /// </summary>
        /// <returns></returns>
        public ActionResult RG01()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "RG01";//員工外出異常表
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

            ViewData["ReportForm"] = ReportForm;

            return View();
        }




        /// <summary>
        /// RG01 外出異常表
        /// </summary>
        /// <param name="ReportForm"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRG01ToExcel(ReportFormModel ReportForm)
        {
            //初始****
            if (ReportForm.EmpNo == null) ReportForm.EmpNo = "";
            if (ReportForm.Company == null) ReportForm.Company = "";
            if (ReportForm.DepartMentNo == null) ReportForm.DepartMentNo = "";
            if (ReportForm.EndDate == null) ReportForm.EndDate = "";
            if (ReportForm.StartDate == null) ReportForm.StartDate = "";






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


                using (MemoryStream msExcel = ModelFactory.RG01ToExcel(ReportForm) as MemoryStream)// 新增試算表
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
