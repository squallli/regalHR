using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHRModel;
using RegalHR.ModelFactory;
using Newtonsoft.Json;
namespace RegalHR.Controllers
{
    public class RegalBonController : Controller
    {

        RegalBonModelFactory ModelFactory = new RegalBonModelFactory();


        /*
        public ActionResult Index()
        {



            return View();
        }
        */


        [HttpGet]
        public ActionResult OutgoingInfo(string EmpNo)
        {

            EmpOutingInfoModel EmpOutingInfo = ModelFactory.GetOutgoingList(EmpNo);

            ViewData["EmpOutingInfo"] = EmpOutingInfo;



            return View();
        }



        public JsonResult UpdateOutgoing(string OutMan, string OutIdAry)
        {
            try
            {

                ModelFactory.UpdateOutgoing(OutMan, OutIdAry);
            }
            catch
            {

            }
            

            return Json("1");
        }


        [HttpPost]
        public JsonResult Login(string CardNo)
        {

            try
            {
                string EmpNo = "";//登入者-員工編號
                EmpNo = ModelFactory.GetLoginEmpNo(CardNo);

                return Json("1|"+EmpNo);
            }
            catch (Exception ex)
            {
                return Json("0|" + ex.Message);
            }
            catch
            {
                return Json("0|系統發生錯誤!");
            }
            


        }

    }
}
