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
    public class EmpController : BaseController
    {

        EmpModelFactory ModelFactory = new EmpModelFactory();

        string UserFreeStyle_id = "EmpGridViewer";


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "E001";
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
                        SearchText2 = "", //性別
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
                    SearchText2 = "", //性別
                };

            }

            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;

            return View();
        }

        [HttpPost]
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../Emp/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            //初始化
            int RecordTotal = 0;
            List<EmpModel> DataList = new List<EmpModel>();

            
            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());

            string SQLWhere ="";

            SQLWhere += UserFreeStyle.SearchCompany != "" ? " AND Company = '" + UserFreeStyle.SearchCompany + "' " : "";

            SQLWhere += UserFreeStyle.SearchDepartMentNo != "" ? " AND DepartMentNo = '" + UserFreeStyle.SearchDepartMentNo + "' " : "";

            SQLWhere += UserFreeStyle.SearchEmpNo != "" ? " AND EmployeeNo = '" + UserFreeStyle.SearchEmpNo + "' " : "";

            SQLWhere += UserFreeStyle.SearchEmpStatus != "" ? " AND Status = '" + UserFreeStyle.SearchEmpStatus + "' " : "";
            
            SQLWhere += UserFreeStyle.SearchText != "" ? " AND (EmployeeNo LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeEName LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeName LIKE '%" + UserFreeStyle.SearchText+ "%' OR CardNo = '" + UserFreeStyle.SearchText + "' ) " : "";

            //性別
            SQLWhere += UserFreeStyle.SearchText2 != "" ? " AND Sex = '" + UserFreeStyle.SearchText2 + "' " : "";

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
        public JsonResult GetEmpList(string FulltimeFlag, string Sex, string Company, string DepartMentNo, string status)
        {
            List<EmpModel> EmpList = null;

            EmpList = ModelFactory.GetEmpList(FulltimeFlag, Sex,Company, DepartMentNo, status);

            return Json(EmpList);
        }

        [HttpPost]
        public JsonResult EmpFormEdit(string EmpJson)
        {

            EmpModel Emp = null;
            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);

                if (Emp.CardStatus == "1")
                {
                    //檢查日期格式 正不正確
                    if (!GeneralObj.CheckDate(Emp.CardEffectiveDate))
                    {
                        throw new Exception("卡號生效日格式有誤!");
                    }


                    if (!GeneralObj.CheckDate(Emp.CardExpiryDate))
                    {
                        throw new Exception("卡號失效日格式有誤!");
                    }

                    Emp.CardEffectiveDate = Emp.CardEffectiveDate.Replace("-", "");
                    Emp.CardExpiryDate = Emp.CardExpiryDate.Replace("-", "");
                }

                if (ModelFactory.EmpEdit(Emp))
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



        /*
        [HttpPost]
        public JsonResult EmpFormAdd(string EmpJson)
        {

            EmpModel Emp = null;

            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);


                Emp.CardEffectiveDate = Emp.CardEffectiveDate.Replace("-", "");
                Emp.CardExpiryDate = Emp.CardExpiryDate.Replace("-", "");


                if (ModelFactory.EmpAdd(Emp))
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
        */

        [HttpPost]
        public JsonResult DeleteCardNo(string EmpJson)
        {

            EmpModel Emp = null;

            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);


                if (ModelFactory.DeleteCardNo(Emp))
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
        public JsonResult InsertCardNo(string EmpJson, string NewCardNo)
        {

            EmpModel Emp = null;

            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);


                if (ModelFactory.InsertCardNo(Emp, NewCardNo))
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

        public ActionResult EmpForm()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "E001";
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

            try
            {
                    EmpModel Emp = ModelFactory.GetEmpData(UserFreeStyle.SignId);
                    Emp.Mode = "EDIT"; //修改模式
                    ViewData["Emp"] = Emp;
            }
            catch(Exception ex)
            {
                    
            }
            catch
            {

            }
            return View();

        }











    }
}
