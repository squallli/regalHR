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
    public class EmpGroupController : BaseController
    {

        EmpGroupModelFactory ModelFactory = new EmpGroupModelFactory();

        string UserFreeStyle_id = "EmpGroupGridViewer";

        public ActionResult Index()
        {

            ViewData["LoginUserInfo"] = LoginUserInfo;


            string ProgramID = "G002";
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
                        SearchCompany = GeneralObj.SearchCompanyDefault(LoginUserInfo,SysProgram),
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









        public ActionResult EmpGroupForm()
        {

            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ProgramID = "G002";
            if (!GeneralObj.CheckProgID(LoginUserInfo.ProgramList, ProgramID))
            {
                return View("../SysMsg/NoAccess");
            }
            else
            {
                ViewData["SysProgram"] = GeneralObj.GetProgram(LoginUserInfo.ProgramList, ProgramID);
            }


            UserFreeStyleModel UserFreeStyle;
            UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());




            EmpGroupModel EmpGroup = ModelFactory.GetEmpGroup(UserFreeStyle.SignId);

            ViewData["EmpGroup"] = EmpGroup;

            return View();
        }





        [HttpPost]
        public JsonResult EmpGroupSave(string EmpGroupJson)
        {

            EmpGroupModel EmpGroup = null;

            try
            {
                EmpGroup = JsonConvert.DeserializeObject<EmpGroupModel>(EmpGroupJson);

                
                

                if (ModelFactory.EmpGroupSave(EmpGroup))
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
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../EmpGroup/GridViewer";
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




    }
}
