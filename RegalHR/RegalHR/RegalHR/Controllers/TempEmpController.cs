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
    public class TempEmpController : BaseController
    {

        TempEmpModelFactory ModelFactory = new TempEmpModelFactory();

        string UserFreeStyle_id = "TempEmpGridViewer";


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;





            string ProgramID = "E002";
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
                        SearchText = "", //搜尋關鍵字
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
                    SearchText = "", //搜尋關鍵字
                };
            }


            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;


            return View();
        }




        
        [HttpPost]
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../TempEmp/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            
            
            //初始化
            int RecordTotal = 0;
            List<EmpModel> DataList = new List<EmpModel>();

            
            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());


            string SQLWhere = "";

            SQLWhere += UserFreeStyle.SearchCompany != "" ? " AND Company = '" + UserFreeStyle.SearchCompany + "' " : "";

            SQLWhere += UserFreeStyle.SearchDepartMentNo != "" ? " AND DepartMentNo = '" + UserFreeStyle.SearchDepartMentNo + "' " : "";


            SQLWhere += UserFreeStyle.SearchEmpStatus != "" ? " AND Status = '" + UserFreeStyle.SearchEmpStatus + "' " : "";

            SQLWhere += UserFreeStyle.SearchText != "" ? " AND (EmployeeNo LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeEName LIKE '%" + UserFreeStyle.SearchText + "%' OR EmployeeName LIKE '%" + UserFreeStyle.SearchText+ "%' OR CardNo = '" + UserFreeStyle.SearchText + "' ) " : "";

            SQLWhere += UserFreeStyle.SearchEmpNo != "" ? " AND EmployeeNo = '" + UserFreeStyle.SearchEmpNo + "' " : "";
            

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
        public JsonResult GetTempEmpList(string DepartMentNo)
        {
            List<EmpModel> EmpList = null;

            EmpList = ModelFactory.GetTempEmpList(DepartMentNo);

            return Json(EmpList);
        }


        


        [HttpPost]
        public JsonResult TempEmpFormEdit(string EmpJson)
        {

            EmpModel Emp = null;


            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);
                if (Emp.CardStatus=="1")
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



                if (ModelFactory.TempEmpEdit(Emp))
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
        public JsonResult TempEmpFormAdd(string EmpJson)
        {

            EmpModel Emp = null;

            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);

                if (Emp.CardNo != null && Emp.CardNo !="")
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




                if (ModelFactory.TempEmpAdd(Emp))
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





        [HttpPost]
        public JsonResult TempEmpFormTurn(string TurnEmpNo, string EmpJson)
        {

            EmpModel Emp = null;

            try
            {
                Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);


                //2016.03.22 修正轉正職一定要卡號問題
                if (Emp.CardStatus=="1")
                {
                    Emp.CardEffectiveDate = Emp.CardEffectiveDate.Replace("-", "");
                    Emp.CardExpiryDate = Emp.CardExpiryDate.Replace("-", "");
                }




                //轉正職
                if (ModelFactory.TurnFullTime(TurnEmpNo, Emp))
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
        


        /// <summary>
        /// 取得需要轉正職的員工
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTurnEmpList(string EmpJson)
        {
            List<EmpModel> EmpList = null;


            EmpModel Emp = JsonConvert.DeserializeObject<EmpModel>(EmpJson);

            EmpList = ModelFactory.GetTurnEmpList(Emp);

            return Json(EmpList);
        }




        [HttpPost]
        public JsonResult TempEmpFormDel(string TempEmpID)
        {

            try
            {

                if (ModelFactory.TempEmpDelete(TempEmpID))
                {

                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("無法註銷!");//失敗
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



        public ActionResult TempEmpForm()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;



            string ProgramID = "E002";
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
                if (UserFreeStyle.SignId == "")
                {
                    EmpModel Emp = new EmpModel();
                    Emp.Mode = "ADD"; //新增模式

                    Emp.EmployeeNo = ModelFactory.GetNewTempEmpNo();
                    Emp.CardNo = "";
                    Emp.CardExpiryDate = "";
                    Emp.CardEffectiveDate = "";
                    Emp.EmployeeEName = "";
                    Emp.EmployeeName = "";
                    Emp.DepartMentNo = "";
                    Emp.Company = LoginUserInfo.Company;

                    ViewData["Emp"] = Emp;
                }
                else
                {
                    EmpModel Emp = ModelFactory.GetTempEmpData(UserFreeStyle.SignId);
                    Emp.Mode = "EDIT"; //修改模式
                    ViewData["Emp"] = Emp;
                }

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
