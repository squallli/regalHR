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
using System.Threading;

namespace RegalHR.ModelFactory
{
    public class HRMailModelFactory
    {
        public DataAccess DbAccess = new DataAccess();

        public HRMailModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }

        /// <summary>
        /// 寄出異常E-Mail
        /// </summary>
        public void AbnSendMail()
        {

            List<AttendanceModel> AttAbnList = GetAttAbnList();

            var tmpList = from tmp in AttAbnList
                          group tmp by new { tmp.EmployeeNo, tmp.EmployeeEName } into g
                          select new { EmployeeNo = g.Key.EmployeeNo, EmployeeEName = g.Key.EmployeeEName };

            string Html = "";


            //＊＊＊ 寄給個人「個人出勤異常通知」  ＊＊＊
            foreach (var tmp in tmpList)
            {
                DataTable dt = DbAccess.ExecuteDataTable("SELECT mail FROM LDAPUsers WHERE description=@description AND mail <> ''",
                     new DbParameter[] {
                     DataAccess.CreateParameter("description", DbType.String, tmp.EmployeeNo.ToString())
                     }
                );

                if (dt.Rows.Count == 1)
                {
                    //代表有找到對應的 E-MAIL  - 寄出信件

                    var tmpAttAbnList = (from tmpAttAbn in AttAbnList
                                         where tmpAttAbn.EmployeeNo == tmp.EmployeeNo
                                         select tmpAttAbn).ToList();

                    Html = "<h2>Dear " + tmp.EmployeeEName + " :</h2> ";

                    Html += "<h3>系統寄發時間 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "</h3> ";
                    Html += "<h3>如對於出勤有任何疑問，請於當日中午前，向財務部提出說明。</h3>";

                    Html += "<table width='100%' border='1'>";
                    Html += "<tr>";
                    Html += "<th align='center'>員工編號</th>";
                    Html += "<th align='center'>部門</th>";
                    Html += "<th align='center'>姓名</th>";
                    Html += "<th align='center'>英文名</th>";
                    Html += "<th align='center'>上班日期</th>";
                    Html += "<th align='center'>上班時間</th>";
                    Html += "<th align='center'>下班時間</th>";
                    Html += "<th align='center'>加班上班</th>";
                    Html += "<th align='center'>加班下班</th>";
                    Html += "<th align='center'>出勤描述(上午)</th>";
                    Html += "<th align='center'>出勤描述(下午)</th>";
                    Html += "<th align='center'>出勤描述(加班)</th>";
                    Html += "<th align='center'>遲到</th>";
                    Html += "</tr>";

                    foreach (var tmp2 in tmpAttAbnList)
                    {
                        Html += "<tr>";
                        Html += "<td align='center'>" + tmp2.EmployeeNo + "</td>";
                        Html += "<td align='center'>" + tmp2.DepartMentName + "</td>";
                        Html += "<td align='center'>" + tmp2.EmployeeName + "</td>";
                        Html += "<td align='center'>" + tmp2.EmployeeEName + "</td>";
                        Html += "<td align='center'>" + tmp2.WorkDay + "</td>";

                        Html += "<td align='center'>" + tmp2.StartWorkTime + "</td>";
                        Html += "<td align='center'>" + tmp2.EndWorkTime + "</td>";
                        Html += "<td align='center'>" + tmp2.StartWorkOvertime + "</td>";
                        Html += "<td align='center'>" + tmp2.EndWorkOvertime + "</td>";
                        //Html += "<td align='center'>" + tmp2.AttendanceDesc + "</td>";

                        // Scott 1129 修改 將出勤欄位 分為 上午、下午、加班 3欄
                        Html += "<td align='center'>" + tmp2.AttendanceDescM + "</td>";
                        Html += "<td align='center'>" + tmp2.AttendanceDescN + "</td>";
                        Html += "<td align='center'>" + tmp2.AttendanceDescOT + "</td>";
                        Html += "<td align='center'>" + tmp2.LateMin + "</td>";
                        Html += "</tr>";
                    }
                    Html += "</table>";

                    try
                    {
                        //Mail_Send(new string[] { dt.Rows[0]["mail"].ToString() }, new string[] { }, "個人出勤異常通知!", Html);
                        Mail_Send(new string[] { "scott.chen@regalscan.com.tw" }, new string[] { }, "個人出勤異常通知!", Html);
                        RegalLib.LogOutput("個人出勤異常通知-寄出成功! 員工編號:" + tmp.EmployeeNo+",E-Mail:"+dt.Rows[0]["mail"].ToString(), "AbnSendMail");

                    }
                    catch(Exception ex)
                    {
                        RegalLib.LogOutput("個人出勤異常通知-寄出失敗! 員工編號:" + tmp.EmployeeNo + ",E-Mail:" + dt.Rows[0]["mail"].ToString() +",ErrMsg:"+ ex.Message, "AbnSendMail");
                    }


                }
                else
                {
                    RegalLib.LogOutput("個人出勤異常通知-寄出失敗! 員工編號:" + tmp.EmployeeNo +" E-Mail有誤!", "AbnSendMail");

                }//if end

            }//foreach end


            //＊＊＊ 寄給天使「員工出勤異常清單」 ＊＊＊
            Html = "<h2>Dear Angel:</h2> ";

            Html += "<h3>系統寄發時間 : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</h3> ";

            Html += "<table width='100%' border='1'>";

            Html += "<tr>";
            Html += "<th align='center'>員工編號</th>";
            Html += "<th align='center'>部門</th>";
            Html += "<th align='center'>姓名</th>";
            Html += "<th align='center'>英文名</th>";
            Html += "<th align='center'>上班日期</th>";
            Html += "<th align='center'>上班時間</th>";
            Html += "<th align='center'>下班時間</th>";
            Html += "<th align='center'>加班上班</th>";
            Html += "<th align='center'>加班下班</th>";
            Html += "<th align='center'>出勤描述(上午)</th>";
            Html += "<th align='center'>出勤描述(下午)</th>";
            Html += "<th align='center'>出勤描述(加班)</th>";
            Html += "<th align='center'>遲到</th>";
            Html += "</tr>";

            foreach (var tmp in AttAbnList)
            {
                Html += "<tr>";
                Html += "<td align='center'>" + tmp.EmployeeNo + "</td>";
                Html += "<td align='center'>" + tmp.DepartMentName + "</td>";
                Html += "<td align='center'>" + tmp.EmployeeName + "</td>";
                Html += "<td align='center'>" + tmp.EmployeeEName + "</td>";
                Html += "<td align='center'>" + tmp.WorkDay + "</td>";

                Html += "<td align='center'>" + tmp.StartWorkTime + "</td>";
                Html += "<td align='center'>" + tmp.EndWorkTime + "</td>";
                Html += "<td align='center'>" + tmp.StartWorkOvertime + "</td>";
                Html += "<td align='center'>" + tmp.EndWorkOvertime + "</td>";
                //Html += "<td align='center'>" + tmp.AttendanceDesc + "</td>";
                // Scott 1129 修改 將出勤欄位 分為 上午、下午、加班 3欄
                Html += "<td align='center'>" + tmp.AttendanceDescM + "</td>";
                Html += "<td align='center'>" + tmp.AttendanceDescN + "</td>";
                Html += "<td align='center'>" + tmp.AttendanceDescOT + "</td>";
                Html += "<td align='center'>" + tmp.LateMin + "</td>";
                Html += "</tr>";
            }
            Html += "</table>";

            try
            {
                //Mail_Send(new string[] { "angel@regalscan.com.tw" }, new string[] { }, "員工出勤異常清單!", Html);
                Mail_Send(new string[] { "scott.chen@regalscan.com.tw" }, new string[] { }, "員工出勤異常清單!", Html);
                RegalLib.LogOutput("員工出勤異常清單-寄出成功!", "AbnSendMail");
            }
            catch
            {
                RegalLib.LogOutput("員工出勤異常清單-寄出失敗!", "AbnSendMail");
            }
        }


        /// <summary>
        /// 取得今日及昨日的出勤異常員工清單
        /// </summary>
        /// <returns></returns>
        public List<AttendanceModel> GetAttAbnList()
        {

            List<AttendanceModel> AttendanceList = new List<AttendanceModel>();

            try
            {

                DataTable dt = DbAccess.ExecuteDataTable("EXEC SearchAttendanceTemp @ManageFlag,@Company,@DepNo,@EmpNo,@KeyWord,@SDATE,@EDATE,@EmpStatus,@LimitVal,@SkipVal",
                    new DbParameter[] {
                    DataAccess.CreateParameter("ManageFlag", DbType.Int16, "1"),
                    DataAccess.CreateParameter("Company", DbType.String, "TPE"),
                    DataAccess.CreateParameter("DepNo", DbType.String,""),
                    DataAccess.CreateParameter("EmpNo", DbType.String, ""),
                    DataAccess.CreateParameter("SDATE", DbType.String, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")),
                    DataAccess.CreateParameter("EDATE", DbType.String, DateTime.Now.ToString("yyyy-MM-dd")),
                    DataAccess.CreateParameter("EmpStatus", DbType.String, "1"),
                    DataAccess.CreateParameter("LimitVal", DbType.Int32, "100000"),
                    DataAccess.CreateParameter("SkipVal", DbType.Int32, "0"),
                    DataAccess.CreateParameter("KeyWord", DbType.String, "")
                    }
                );

                //查看是否有曠職
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    //過濾掉 陳先生與TIMO
                    if (dt.Rows[i]["EmployeeNo"].ToString() == "00001" || dt.Rows[i]["EmployeeNo"].ToString() == "00165")
                    {
                        continue;
                    }

                    //判斷今日是否為上班日   若為非上班日則跳回
                    if (!(dt.Rows[i]["AttendanceDesc2"].ToString().IndexOf("非上班日") == -1 ))
                    {
                        continue; 
                    }

                    
                    //判斷是否為曠職  若沒有曠職則跳回
                    if (dt.Rows[i]["AttendanceDesc"].ToString().IndexOf("曠職")==-1)
                    {
                        continue;
                    }

                    AttendanceList.Add(new AttendanceModel()
                    {
                        EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString(),
                        DepartMentName = dt.Rows[i]["DepartMentName"].ToString(),
                        EmployeeName = dt.Rows[i]["EmployeeName"].ToString(),
                        EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString(),
                        CompanyName = dt.Rows[i]["CompanyName"].ToString(),
                        StartWorkTime = dt.Rows[i]["StartWorkTime"].ToString(),
                        EndWorkTime = dt.Rows[i]["EndWorkTime"].ToString(),
                        WorkDay = dt.Rows[i]["WorkDay"].ToString(),
                        LateMin = dt.Rows[i]["LateMin"].ToString(),
                        LeaveMin = dt.Rows[i]["LeaveMin"].ToString(),
                        StartWorkOvertime = dt.Rows[i]["StartWorkOvertime"].ToString(),
                        EndWorkOvertime = dt.Rows[i]["EndWorkOvertime"].ToString(),
                        AttendanceDescM = dt.Rows[i]["AttendanceDescM"].ToString(),
                        AttendanceDescN = dt.Rows[i]["AttendanceDescN"].ToString(),
                        AttendanceDescOT = dt.Rows[i]["AttendanceDescOT"].ToString(),
                        LateMinFormat = dt.Rows[i]["LateMinFormat"].ToString(),
                        AttendanceDesc2 = dt.Rows[i]["AttendanceDesc2"].ToString(),

                        RecordLimit = dt.Rows[i]["RecordLimit"].ToString(),
                        RowNumID = dt.Rows[i]["RowNumID"].ToString(),
                        RowNumTotal = dt.Rows[i]["RowNumTotal"].ToString(),
                    });
                }//for end
            }
            catch
            {
                throw new Exception("無法取得清單");
            }

            return AttendanceList;
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

            string MailFrom = "帝商出勤系統<APSERVICE@regalscan.com.tw>";

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
                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))//或公司、客戶的smtp_server
                {
                    if (!string.IsNullOrEmpty(mailAccount) && !string.IsNullOrEmpty(mailPwd))//.config有帳密的話
                    {
                        client.Credentials = new NetworkCredential(mailAccount, mailPwd);//寄信帳密
                    }
                    client.Send(mms);//寄出一封信
                }//end using 

                //釋放每個附件，才不會Lock住
                if (mms.Attachments != null && mms.Attachments.Count > 0)
                {
                    for (int i = 0; i < mms.Attachments.Count; i++)
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