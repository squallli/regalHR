using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;
using System.Net.Mail;
using System.Net;
namespace RegalHR.ModelFactory
{
    public class RegalBonModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public RegalBonModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }



        /// <summary>
        /// 取得該員工的外出資料
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public EmpOutingInfoModel GetOutgoingList(string EmpNo)
        {
            EmpOutingInfoModel EmpOutingInfo = new EmpOutingInfoModel();
            string TempEmpNo = "";//臨時員工編號
            



            //先取得員工基本資料

            DataTable dtEmpInfo = DbAccess.ExecuteDataTable("SELECT EmployeeEName,EmployeeName,EmployeeNo FROM gvEmployeeAll WHERE EmployeeNo=@EmpNo ",
               new DbParameter[] {
                                    DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo),
                                }
                );

            if (dtEmpInfo.Rows.Count == 0)
            {
                throw new Exception("查無此員工資料");
            }
            else
            {
                EmpOutingInfo.EmployeeNo = dtEmpInfo.Rows[0]["EmployeeNo"].ToString();
                EmpOutingInfo.EmployeeName = dtEmpInfo.Rows[0]["EmployeeName"].ToString();
                EmpOutingInfo.EmployeeEName = dtEmpInfo.Rows[0]["EmployeeEName"].ToString();
            }






            //先取得臨時員工編號
            DataTable dtTempEmpNo= DbAccess.ExecuteDataTable("SELECT EmployeeNo FROM tbTempEmployee WHERE  FullTimeEmployeeNo = @FullTimeEmployeeNo",
               new DbParameter[] {
                                    DataAccess.CreateParameter("FullTimeEmployeeNo", DbType.String, EmpNo),
                                }
                );

            if (dtTempEmpNo.Rows.Count!=0)
            {
                TempEmpNo = dtTempEmpNo.Rows[0]["EmployeeNo"].ToString();
            }








            //賦外出日期
            EmpOutingInfo.OutgoingDate = DateTime.Now.ToString("yyyy-MM-dd");

            //取得外出資料
            DataTable dtOutgoing = DbAccess.ExecuteDataTable("SELECT OutId,left(OutTime,2)+':'+right(OutTime,2) as OutTime,Location,CustomerName,left(GoOutTime,2)+':'+right(GoOutTime,2) as GoOutTime,UpdateDate,UpdateTime FROM gvOutgoing WHERE Status IN ('A','U','AM','UM') AND OutDate=@OutDate AND OutMan IN (@OutMan,@TempEmpNo) AND (GoOutTime IS NULL OR GoOutTime = '') Order by OutTime",
               new DbParameter[] {
                                    DataAccess.CreateParameter("OutDate", DbType.String, DateTime.Now.ToString("yyyyMMdd") ),
                                    DataAccess.CreateParameter("OutMan", DbType.String, EmpNo),
                                    DataAccess.CreateParameter("TempEmpNo", DbType.String, TempEmpNo),
                                }
                );






            for (int i = 0; i < dtOutgoing.Rows.Count;i++ )
            {
                EmpOutingInfo.OutgoingList.Add(
                    new OutgoingListModel()
                    {
                        OutId = dtOutgoing.Rows[i]["OutId"].ToString(),
                        OutTime = dtOutgoing.Rows[i]["OutTime"].ToString(),
                        Location = dtOutgoing.Rows[i]["Location"].ToString(),
                        CustomerName = dtOutgoing.Rows[i]["CustomerName"].ToString(),
                        GoOutTime = dtOutgoing.Rows[i]["GoOutTime"].ToString(),
                        UpdateDate = dtOutgoing.Rows[i]["UpdateDate"].ToString(),
                        UpdateTime = dtOutgoing.Rows[i]["UpdateTime"].ToString()
                    }

                 );
            }

            return EmpOutingInfo;
        }



        













        /// <summary>
        /// 登入驗證
        /// </summary>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public string GetLoginEmpNo(string CardNo)
        {
            DataTable dt = null;


            /*
            if (CardNo.Length == 5)
            {

                
                //產生臨時編號

                //判定是否有此人及此帳號
                dt = DbAccess.ExecuteDataTable("SELECT mail,givenName FROM LDAPUsers WHERE description=@EmpNo ",
                        new DbParameter[] {
                                    DataAccess.CreateParameter("EmpNo", DbType.String, CardNo),
                                }
                    );


                if (dt.Rows.Count == 0)
                {
                    throw new Exception("無法產生臨時編號..查無此員工!");
                }
                else
                {
                    TempLoginModel TempLogin = GetTempLogin(CardNo);//取得臨時編號



                    string Html = "<h2>Dear " + dt.Rows[0]["givenName"].ToString() + " :</h2> ";

                    Html += "<h3>您的出勤臨時編號為:" + TempLogin.Rid + " , 有效時間到: " + TempLogin.EndTime + " </h3>";

                    Mail_Send(new string[] { dt.Rows[0]["mail"].ToString() }, new string[] { }, "外出臨時登入編號",Html);

                    throw new Exception("系統已寄出臨時登入編號至您的信箱");
                    

                }
                

            }
            */



            if (CardNo.Length == 4)
            {
                //臨時登入編號四碼
                dt = DbAccess.ExecuteDataTable("SELECT EmpNo FROM tbTempLogin where  EndTime >= @Now AND Rid=@Rid ",
                     new DbParameter[] {
                                    DataAccess.CreateParameter("Rid", DbType.String, CardNo),
                                    DataAccess.CreateParameter("Now", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                                }
                    );



            }
            else
            {

                //員工卡號

                dt = DbAccess.ExecuteDataTable("SELECT EmployeeNo AS EmpNo FROM tbCard WHERE Status='1' AND CardNo=@CardNo  ",
                       new DbParameter[] {
                            DataAccess.CreateParameter("CardNo", DbType.String, CardNo),                    
                        }
                     );
            }



            if (dt.Rows.Count!=1)
            {
                throw new Exception("無效的卡號");
            }





            return dt.Rows[0]["EmpNo"].ToString();
        }





        public void UpdateOutgoing(string OutMan,string OutIdAry)
        {
            if (OutIdAry == "" || OutMan=="")
            {
                //無任何更新
                return;
            }


            //先取得gvOutgoing 最後一筆資料
            DataTable dtOutgoing = DbAccess.ExecuteDataTable("SELECT OutId,OutMan,UpdateDate,UpdateTime FROM gvOutgoing WHERE OutId IN (" + OutIdAry + ") AND OutMan=@OutMan ",
               new DbParameter[] {
                                DataAccess.CreateParameter("OutMan", DbType.String, OutMan)
                            }
                );







             DbTransaction objTrans = DbAccess.CreateDbTransaction();


             try
             {


                 for (int i = 0; i < dtOutgoing.Rows.Count; i++)
                 {
                     DbAccess.ExecuteNonQuery("UPDATE Outgoing SET GoOutTime=@GoOutTime WHERE OutId=@OutId AND OutMan=@OutMan AND UpdateTime=@UpdateTime AND UpdateDate=@UpdateDate ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("OutId", DbType.String, dtOutgoing.Rows[i]["OutId"].ToString()),
                        DataAccess.CreateParameter("OutMan", DbType.String, dtOutgoing.Rows[i]["OutMan"].ToString()),
                        DataAccess.CreateParameter("UpdateDate", DbType.String,  dtOutgoing.Rows[i]["UpdateDate"].ToString()),
                        DataAccess.CreateParameter("UpdateTime", DbType.String,  dtOutgoing.Rows[i]["UpdateTime"].ToString()),
                        DataAccess.CreateParameter("GoOutTime", DbType.String,  DateTime.Now.ToString("HHmm")),
                    });
                 }

                 objTrans.Commit();
             }
             catch
             {
                 objTrans.Rollback();
             }


        }





        /// <summary>
        /// 取得臨時編號
        /// </summary>
        /// <param name="EmpNo">員工編號</param>
        /// <param name="Flag">是否產生旗標 true 代表臨時編號不存在則幫忙產生一組新的  ,false= 代表臨時編號不存在則不產生</param>
        /// <returns></returns>
        public TempLoginModel GetTempLogin(string EmpNo,bool Flag=true)
        {



            TempLoginModel TempLogin = new TempLoginModel();


            //判定該使用者 是否已經產生過 臨時編號
            DataTable dtExist = DbAccess.ExecuteDataTable("SELECT Rid,EmpNo,CONVERT(char(19), EndTime, 120) AS EndTime,CONVERT(char(19), StartTime, 120) AS StartTime FROM tbTempLogin WHERE EmpNo = @EmpNo AND EndTime >= @EndTime ",
            new DbParameter[] {
                DataAccess.CreateParameter("EmpNo", DbType.String, EmpNo),
                DataAccess.CreateParameter("EndTime", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                }
            );




            if (dtExist.Rows.Count>0)
            {
                TempLogin.EmpNo = dtExist.Rows[0]["EmpNo"].ToString();
                TempLogin.Rid = dtExist.Rows[0]["Rid"].ToString();
                TempLogin.EndTime = dtExist.Rows[0]["EndTime"].ToString();
                TempLogin.StartTime = dtExist.Rows[0]["StartTime"].ToString();
            }
            else
            {

                //產生新的臨時編號
                if (Flag==false)
                {
                    throw new Exception("查無臨時編號");
                }







                //先找到一筆 小於今天日期以前之隨便一組Rid
                DataTable dt = DbAccess.ExecuteDataTable("SELECT TOP 1 Rid FROM tbTempLogin WHERE EndTime < @EndTime ORDER BY NEWID()",
                new DbParameter[] {
                                DataAccess.CreateParameter("EndTime", DbType.String, DateTime.Now.ToString("yyyy-MM-dd")),
                            }
                );


            

                if (dt.Rows.Count == 0)
                {
                    throw new Exception("已無法產生編號");
                }



                TempLogin.Rid =  dt.Rows[0]["Rid"].ToString();
                TempLogin.EmpNo = EmpNo;



                TempLogin.StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                TempLogin.EndTime =  DateTime.Now.AddSeconds(30).ToString("yyyy-MM-dd HH:mm:ss");
                




                DbTransaction objTrans = DbAccess.CreateDbTransaction();

                try
                {


                    DbAccess.ExecuteNonQuery("UPDATE tbTempLogin SET EmpNo=@EmpNo,EndTime=@EndTime,StartTime=@StartTime  WHERE Rid = @Rid  ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("Rid", DbType.String ,TempLogin.Rid),
                        DataAccess.CreateParameter("EmpNo", DbType.String, TempLogin.EmpNo),
                        DataAccess.CreateParameter("StartTime", DbType.String, TempLogin.StartTime),
                        DataAccess.CreateParameter("EndTime", DbType.String, TempLogin.EndTime),
                    });

                    objTrans.Commit();
                }
                catch
                {
                    objTrans.Rollback();
                    throw new Exception("產生臨時編號失敗");
                }


            }

            

            return TempLogin;
        }














    /// <summary>
    /// 完整的寄信功能
    /// </summary>
    /// <param name="MailTos">收信人E-mail Address</param>
    /// <param name="Ccs">副本E-mail Address</param>
    /// <param name="MailSub">主旨</param>
    /// <param name="MailBody">信件內容</param>
    /// <returns>是否成功</returns>
    public bool Mail_Send(string[] MailTos, string[] Ccs, string MailSub, string MailBody)
    {

        string MailFrom = "出勤系統<APSERVICE@regalscan.com.tw>";
        

        int smtpPort = 25;//Port
        string smtpServer = System.Configuration.ConfigurationManager.AppSettings["MailsmtpServer"];// 寄信smtp server
        string mailAccount = System.Configuration.ConfigurationManager.AppSettings["MailAccount"];//寄信帳號
        string mailPwd = System.Configuration.ConfigurationManager.AppSettings["MailPwd"];//寄信密碼

        try
        {

            //建立MailMessage物件
            System.Net.Mail.MailMessage mms = new System.Net.Mail.MailMessage();
            //指定一位寄信人MailAddress
            mms.From = new MailAddress(MailFrom);
            //信件主旨
            mms.Subject = MailSub;
            //信件內容
            mms.Body = MailBody;
            //信件內容 是否採用Html格式
            mms.IsBodyHtml = true;

            if (MailTos != null)//防呆
            {
                for (int i = 0; i < MailTos.Length; i++)
                {
                    //加入信件的收信人(們)address
                    if (!string.IsNullOrEmpty(MailTos[i].Trim()))
                    {
                        mms.To.Add(new MailAddress(MailTos[i].Trim()));
                    }

                }
            }//End if (MailTos !=null)//防呆

            if (Ccs != null) //防呆
            {
                for (int i = 0; i < Ccs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Ccs[i].Trim()))
                    {
                        //加入信件的副本(們)address
                        mms.CC.Add(new MailAddress(Ccs[i].Trim()));
                    }

                }
            }//End if (Ccs!=null) //防呆


             

           using (SmtpClient client = new SmtpClient(smtpServer,smtpPort))//或公司、客戶的smtp_server
                    {
                        if (!string.IsNullOrEmpty(mailAccount) && !string.IsNullOrEmpty(mailPwd))//.config有帳密的話
                        {
                            client.Credentials = new NetworkCredential(mailAccount, mailPwd);//寄信帳密
                        }
                        client.Send(mms);//寄出一封信
                    }//end using 
                    //釋放每個附件，才不會Lock住
                    if (mms.Attachments != null && mms.Attachments.Count>0)
                    {
                        for (int i = 0; i < mms.Attachments.Count;i++ )
                        {
                            mms.Attachments[i].Dispose();
                            mms.Attachments[i] = null;
                        } 
                    }



            return true;//成功
        }
        catch (Exception ex)
        {
            return false;//寄失敗
        }
    }


    }
}