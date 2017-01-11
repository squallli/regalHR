using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHR.ModelFactory;
using RegalHRModel;
using Newtonsoft.Json;

namespace RegalHR.Controllers
{
    public class HolidayController : BaseController
    {

        HolidayModelFactory ModelFactory = new HolidayModelFactory();

        string UserFreeStyle_id = "HolidayGridViewer";



        [HttpPost]
        public JsonResult EditHolidayDtl(string HolidayJson)
        {


            HolidayDtlModel HolidayDtl = null;

            try
            {

                HolidayDtl = JsonConvert.DeserializeObject<HolidayDtlModel>(HolidayJson);

                if (ModelFactory.EditHolidayDtl(HolidayDtl))
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



        [HttpPost]
        public JsonResult DeleteHolidayDtl(string HolidayJson)
        {


            HolidayDtlModel HolidayDtl = null;
            try
            {

                HolidayDtl = JsonConvert.DeserializeObject<HolidayDtlModel>(HolidayJson);

                if (ModelFactory.DeleteHolidayDtl(HolidayDtl))
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

        
        

        [HttpPost]
        public JsonResult InsertHolidayDtl(string HolidayJson)
        {


            HolidayDtlModel HolidayDtl = null;
            try
            {

                HolidayDtl = JsonConvert.DeserializeObject<HolidayDtlModel>(HolidayJson);


                HolidayDtl.Holiday = HolidayDtl.Holiday.Replace("-", "");



                if (ModelFactory.InsertHolidayDtl(HolidayDtl))
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



        [HttpPost]
        public JsonResult HolidayFormAdd(string Company, string YearId)
        {

            try
            {


                if (ModelFactory.InsertHoliday(Company, YearId))
                {

                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("");//失敗
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






        [HttpPost]
        public JsonResult HolidayFormCopy(string Company, string YearId, string CopyId)
        {

            try
            {


                if (ModelFactory.CopyHoliday(Company, YearId, CopyId))
                {

                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("");//失敗
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





        [HttpPost]
        public JsonResult HolidayFormDel(string Company, string YearId)
        {

            try
            {


                if (ModelFactory.DeleteHoliday(Company, YearId))
                {

                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("");//失敗
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

        



        public ActionResult HolidayForm()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;



            string ProgramID = "Z001";
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


            HolidayModel Holiday = ModelFactory.GetHolidayData(UserFreeStyle.SignId);

            ViewData["Holiday"] = Holiday;

            return View();
        }




        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "Z001";
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
                        OrderField = "YearId",
                        OrderType = "DESC",
                        PageNum = "0",
                        SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                        SearchText = "", //搜尋狀態
                    };
                    
                }
            }
            catch
            {
                UserFreeStyle = new UserFreeStyleModel()
                {
                    Name = UserFreeStyle_id,
                    OrderField = "YearId",
                    OrderType = "DESC",
                    PageNum = "0",
                    SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo, SysProgram),
                    SearchText = "", //搜尋狀態
                };

            }


            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;



            return View();
        }




        
        [HttpPost]
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../Holiday/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            
            
            //初始化
            int RecordTotal = 0;
            List<HolidayModel> DataList = new List<HolidayModel>();

            
            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());


            string SQLWhere ="";

            SQLWhere += UserFreeStyle.SearchCompany != "" ? " AND Company = '" + UserFreeStyle.SearchCompany + "' " : "";

            SQLWhere += UserFreeStyle.SearchText != "" ? " AND (YearId LIKE '%" + UserFreeStyle.SearchText + "%' OR CompanyName LIKE '%" + UserFreeStyle.SearchText + "%'  ) " : "";



            string SQLOrderby =  UserFreeStyle.OrderField + " " + UserFreeStyle.OrderType;
 


            //賦予資料清單及資料總筆數
            ModelFactory.GetGridViewList( SQLWhere, SQLOrderby, PageNum, GeneralObj.GridViewerLimit, ref DataList, ref RecordTotal);

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










        [HttpPost]
        public JsonResult GetEventList(string Company,string YearId,string SDATE, string EDATE)
        {

            List<HolidayDtlModel> M = ModelFactory.GetEventList(Company, YearId, SDATE, EDATE);
            return Json(M);
        }


    }
}
