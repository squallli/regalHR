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
    public class GroupController : BaseController
    {

        GroupModelFactory ModelFactory = new GroupModelFactory();

        string UserFreeStyle_id = "GroupGridViewer";


        public ActionResult Index()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;



            string ProgramID = "G001";
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
                        OrderField ="GroupID",
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
                    OrderField = "GroupID",
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
            string TargetURL = "../Group/GridViewer";
            GridViewerModel GViewer = new GridViewerModel();

            
            
            //初始化
            int RecordTotal = 0;
            List<GroupModel> DataList = new List<GroupModel>();


            UserFreeStyleModel UserFreeStyle = JsonConvert.DeserializeObject<UserFreeStyleModel>(Session[UserFreeStyle_id].ToString());


            string SQLWhere = " AND GroupID LIKE '" + UserFreeStyle.SearchText + "%' OR GroupName LIKE '" + UserFreeStyle.SearchText + "%' ";

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



        public ActionResult GroupForm()
        {
            ViewData["LoginUserInfo"] = LoginUserInfo;




            string ProgramID = "G001";
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
                    GroupModel Group = new GroupModel();
                    Group.Mode = "ADD"; //新增模式
                    Group.ViewLevel = "C";


                    ProgramModelFactory ProgramModelFactorys = new ProgramModelFactory();
                    Group.GroupProgramList = ProgramModelFactorys.GetGroupProgramList();


                    ViewData["Group"] = Group;
                }
                else
                {
                    GroupModel Group = ModelFactory.GetGroupData(UserFreeStyle.SignId);
                    Group.Mode = "EDIT"; //修改模式
                    ViewData["Group"] = Group;
                }


            }
            catch (Exception ex)
            {


            }
            catch
            {

            }



            return View();
        }















        [HttpPost]
        public JsonResult GroupFormEdit(string GroupJson)
        {

            GroupModel Group = null;


            try
            {
                Group = JsonConvert.DeserializeObject<GroupModel>(GroupJson);


                if (ModelFactory.GroupEdit(Group))
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
        public JsonResult GroupFormAdd(string GroupJson)
        {

            GroupModel Group = null;

            try
            {
                Group = JsonConvert.DeserializeObject<GroupModel>(GroupJson);


                if (ModelFactory.GroupAdd(Group))
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
        public JsonResult GroupFormDel(string GroupID)
        {

            try
            {

                if (ModelFactory.GroupDelete(GroupID))
                {

                    return Json("1");//代表已經處裡完
                }
                else
                {

                    return Json("");//失敗
                }

            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
            catch
            {
                return Json("系統發生錯誤");
            }
        }


    }
}
