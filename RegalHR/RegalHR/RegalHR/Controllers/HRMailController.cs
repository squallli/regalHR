using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegalHR.ModelFactory;
using RegalHRModel;
using Newtonsoft.Json;
using System.Threading;
namespace RegalHR.Controllers
{
    public class HRMailController : Controller
    {
        HRMailModelFactory ModelFactory = new HRMailModelFactory();

        //http://192.168.0.12:81/HRMail/AbnSendMail?key=89947155
        /// <summary>
        /// 出勤異常通知 寄E-mail事件
        /// </summary>
        public EmptyResult AbnSendMail(string Key)
            //(string Key)
        {

            try
            {

                if (Key == "89947155")
                {
                    ThreadStart ts = new ThreadStart(ModelFactory.AbnSendMail);
                    Thread t = new Thread(ts);
                    t.Start();

                    RegalLib.LogOutput("已觸發出勤異常通知!", "AbnSendMail");
                }
                else
                {
                    RegalLib.LogOutput("觸發出勤異常通知時,KEY值錯誤!", "AbnSendMail");
                }
            }
            catch (Exception ex)
            {
                RegalLib.LogOutput("觸發出勤異常通知時,發生問題! " + ex.Message, "AbnSendMail");
            }


            return new EmptyResult();

        }

    }
}
