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
using System.DirectoryServices;
using System.Configuration;
using System.Text;
using RegalEIPLoginKey;

namespace RegalHR.Controllers
{
    public class LoginController : Controller
    {

        LoginModelFactory ModelFactory = new LoginModelFactory();


        public ActionResult Index()
        {

            string ip = Request.ServerVariables["REMOTE_ADDR"];

            if (ip == "::1" || ip.IndexOf("192.168.") != -1 || ip.IndexOf("127.0.0.1") != -1)
            {
                return View();
            }
            else
            {
                Response.Write("目前IP:" + ip +" , 必須使用公司內部網路才能進行登入!");
                return new EmptyResult();
            }
            
            
        }



        public ActionResult LoginOut()
        {
            Session["LoginUserInfo"] = null;
            Response.Redirect("../Login/", true);
            return View();
        }



        /// <summary>
        /// 判定Session存不存在
        /// </summary>
        /// <returns></returns>
        public JsonResult ExistsSession()
        {
            if (Session["LoginUserInfo"] == null)
            {
                return Json("0");
            }
            else
            {
                return Json("1");
            }
        }





        [HttpPost]
        public JsonResult LoginSys(string LoginId,string LoginPwd)
        {
            try
            {
                UserModel LoginUserInfo = ModelFactory.Login(LoginId, LoginPwd);

                if (LoginUserInfo == null)
                {
                    return Json("系統發生錯誤");
                }

                Session["LoginUserInfo"] = JsonConvert.SerializeObject(LoginUserInfo);

                return Json("1");//代表已經處裡完
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




        [HttpGet]
        public EmptyResult AutoLogin()
        {

            //限制只能內網才可以進入
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            if (ip == "::1" || ip.IndexOf("192.168.") != -1 || ip.IndexOf("127.0.0.1") != -1)
            {

            }
            else
            {
                Response.Redirect("../", true);
                return new EmptyResult();
            }




                //清除Session
                if (Session["LoginUserInfo"] != null)
                {


                    Session["LoginUserInfo"] = null;
                }


                string EmpInfo = RegalEIPLoginKey.EIPLogin.GetDecryptString("1234", Request["q"].ToString());

                try
                {

                    string[] EmpInfoAry = EmpInfo.Split(',');

                    if (EmpInfoAry.Count() < 2)
                    {
                        
                        throw new Exception("解碼失敗" );
                    }
                    else
                    {

                        //EmpInfoAry[0] 代表 公號
                        //EmpInfoAry[1] 代表 LDAP帳號

                        UserModel LoginUserInfo = ModelFactory.Login(EmpInfoAry[1], "", true);

                        Session["LoginUserInfo"] = JsonConvert.SerializeObject(LoginUserInfo);

                        Response.Redirect("../", true);
                        return new EmptyResult();
                    }

                }
                catch(Exception ex)
                {
                    Response.Write(EmpInfo);

                    Response.Write(ex.Message);
                    //Response.Redirect("/Login/", true);
                    return new EmptyResult();

                }



        }


    }
}
