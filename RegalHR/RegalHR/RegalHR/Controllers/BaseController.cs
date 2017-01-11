using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHRModel;
using RegalHR.ModelFactory;
using REGAL.Data.DataAccess;
using Newtonsoft.Json;
namespace RegalHR.Controllers
{
    public class BaseController : Controller
    {
        public UserModel LoginUserInfo;
        public GeneralModelFactory GeneralObj = new GeneralModelFactory();
        public DataAccess DbAccess = new DataAccess();


        public BaseController()
        {

            //資料庫連線配置
            
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }


        /// <summary> 執行前先讀取資料 </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            

            if (Session["LoginUserInfo"] == null)
            {
                //直接跳至Login畫面
                //Response.Redirect("/Login/", true);
                //return;


                //Response.StatusCode = 301;
                //Response.AppendHeader("Location", Url.Action("Login"));
                //RedirectToAction("/Login/");

                filterContext.Result = new RedirectResult("/Login/");
                //filterContext.ExceptionHandled = true;
               

            }
            else
            {

                LoginUserInfo = JsonConvert.DeserializeObject<UserModel>(Session["LoginUserInfo"].ToString());
            }


            base.OnActionExecuting(filterContext);
        }



    }
}
