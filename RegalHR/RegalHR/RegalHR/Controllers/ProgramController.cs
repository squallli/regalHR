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
    public class ProgramController : BaseController
    {

        ProgramModelFactory ModelFactory = new ProgramModelFactory();

        string UserFreeStyle_id = "ProgramGridViewer";


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;

            string ip = Request.ServerVariables["REMOTE_ADDR"];

            if (ip.IndexOf("127.0.0.1") != -1)
            {
                Response.Write("您無法使用此作業!");
                return new EmptyResult();
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
                        OrderField ="ProgID",
                        OrderType = "ASC",
                        PageNum = "0" ,
                        SearchText = ""
                    };
                    
                }
            }
            catch
            {
                UserFreeStyle = new UserFreeStyleModel()
                {
                    Name = UserFreeStyle_id,
                    OrderField = "ProgID",
                    OrderType = "ASC",
                    PageNum = "0" ,
                    SearchText = ""
                };

            }


            Session[UserFreeStyle_id] = JsonConvert.SerializeObject(UserFreeStyle);
            ViewData["UserFreeStyle"] = UserFreeStyle;


            return View();
        }




        
        [HttpPost]
        public JsonResult GridViewer(int PageNum)
        {
            string TargetURL = "../Program/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            
            
            //初始化
            int RecordTotal = 0;
            List<ProgramModel> DataList = new List<ProgramModel>();

            
            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());


            string SQLWhere = " AND ProgID LIKE '" + UserFreeStyle.SearchText + "%' OR ProgName LIKE '" + UserFreeStyle.SearchText + "%' OR Power LIKE '" + UserFreeStyle.SearchText + "%'";

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
        public JsonResult ProgramFormEdit(string ProgramJson)
        {

            ProgramModel Program = null;


            try
            {
                Program = JsonConvert.DeserializeObject<ProgramModel>(ProgramJson);


                if (ModelFactory.ProgramEdit(Program))
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
        public JsonResult ProgramFormAdd(string ProgramJson)
        {

            ProgramModel Program = null;

            try
            {
                Program = JsonConvert.DeserializeObject<ProgramModel>(ProgramJson);


                if (ModelFactory.ProgramAdd(Program))
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
        public JsonResult ProgramFormDel(string ProgID)
        {

            try
            {

                if (ModelFactory.ProgramDelete(ProgID))
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









        public ActionResult ProgramForm()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;



            UserFreeStyleModel UserFreeStyle;
            UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());


            try
            {
                if (UserFreeStyle.SignId == "")
                {
                    ProgramModel Program = new ProgramModel();
                    Program.Mode = "ADD"; //新增模式
                    ViewData["Program"] = Program;
                }
                else
                {
                    ProgramModel Program = ModelFactory.GetProgramData(UserFreeStyle.SignId);
                    Program.Mode = "EDIT"; //修改模式
                    ViewData["Program"] = Program;
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
