using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;
using System.DirectoryServices;
using System.Configuration;


namespace RegalHR.ModelFactory
{
    public class LoginModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public LoginModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }



        public UserModel Login(string LoginId, string LoginPwd,bool AutoLogin=false)
        {
            UserModel UserInfo = null;
            try
            {
                
                DataTable dt = DbAccess.ExecuteDataTable("SELECT DomainAccount,Domain,sAMAccountName,sAMAccountType,description,mail,Path,IsOff FROM LDAPUsers WHERE sAMAccountName = @sAMAccountName",
                     new DbParameter[] {
                     DataAccess.CreateParameter("sAMAccountName", DbType.String, LoginId.ToString())
                     }
                );



                if (dt.Rows.Count == 0)
                {
                    throw new Exception("查無此帳號!");
                }else if (dt.Rows.Count >= 2)
                {
                    throw new Exception("該帳戶有兩筆以上的對應");
                }else if (dt.Rows.Count == 1)
                {
                    //LDAP驗證登入!



                    //若是自動登入,則忽略LDAP機制
                    if (!AutoLogin)
                    {
                        DirectoryEntry entry = new DirectoryEntry(ConfigurationManager.AppSettings["LDAP"], LoginId.ToString(), LoginPwd.ToString());
                        DirectorySearcher search = new DirectorySearcher(entry);
                        search.Filter = "(SAMAccountName=" + LoginId + ")";
                        search.PropertiesToLoad.Add("objectGUID");
                        search.PropertiesToLoad.Add("cn");
                        search.PropertiesToLoad.Add("sn");
                        search.PropertiesToLoad.Add("description");
                        search.PropertiesToLoad.Add("mail");
                        search.PropertiesToLoad.Add("usergroup");
                        search.PropertiesToLoad.Add("displayname");
                        search.PropertiesToLoad.Add("memberOf");
  
                        SearchResult result = search.FindOne();
                    }


                    UserInfo = new UserModel();
                    UserInfo.LDAP = new LDAPUsersModel();
                    UserInfo.LDAP.DomainAccount = dt.Rows[0]["DomainAccount"].ToString();
                    UserInfo.LDAP.Domain = dt.Rows[0]["Domain"].ToString();
                    UserInfo.LDAP.sAMAccountName = dt.Rows[0]["sAMAccountName"].ToString();
                    UserInfo.LDAP.sAMAccountType = dt.Rows[0]["sAMAccountType"].ToString();
                    UserInfo.LDAP.description = dt.Rows[0]["description"].ToString();
                    UserInfo.LDAP.mail = dt.Rows[0]["mail"].ToString();
                    UserInfo.LDAP.Path = dt.Rows[0]["Path"].ToString();
                    UserInfo.LDAP.IsOff = dt.Rows[0]["IsOff"].ToString();




                    //ORDER BY GroupID ASC 很重要  會先寫最大權限之作業
                    string SQL = " SELECT EMP.*,G.ViewLevel,G.GroupID,G.Power FROM gvEmployeeAll EMP "
                                +" LEFT OUTER JOIN tbEmpGroup EG ON EMP.EmployeeNo = EG.EmployeeNo "
                                + " LEFT OUTER JOIN tbGroup G ON EG.GroupID = G.GroupID WHERE Emp.EmployeeNo = @EmployeeNo ORDER BY ViewLevel ASC";

                    DataTable dtLoginInfo = DbAccess.ExecuteDataTable(SQL,
                         new DbParameter[] {
                                         DataAccess.CreateParameter("EmployeeNo", DbType.String, UserInfo.LDAP.description.ToString())
                          }
                    );


                    if (dtLoginInfo.Rows.Count == 0)
                    {
                        throw new Exception("此帳戶尚未建立於本系統!");
                    }
                    else
                    {
                        UserInfo.Company = dtLoginInfo.Rows[0]["Company"].ToString().Trim();
                        UserInfo.CompanyName = dtLoginInfo.Rows[0]["CompanyName"].ToString().Trim();
                        UserInfo.UserId = dtLoginInfo.Rows[0]["EmployeeNo"].ToString().Trim();
                        UserInfo.UserName = dtLoginInfo.Rows[0]["EmployeeName"].ToString().Trim();
                        UserInfo.DepNo = dtLoginInfo.Rows[0]["DepartMentNo"].ToString().Trim();
                        UserInfo.DepName = dtLoginInfo.Rows[0]["DepartMentName"].ToString().Trim();
                        UserInfo.UserEName = dtLoginInfo.Rows[0]["EmployeeEName"].ToString().Trim();




                        //回寫此帳戶可使用之作業權限
                        DataTable BaseGroup = DbAccess.ExecuteDataTable("select * from tbProgram WHERE FlagType = '1' ");
                        //DataTable BaseGroup = DbAccess.ExecuteDataTable("select * from tbProgram");


                        //寫入基本預設權限

                        for (int i = 0; i < BaseGroup.Rows.Count; i++)
                        {


                            int TmpTotal = (from tmpList in UserInfo.ProgramList
                                            where tmpList.ProgID == BaseGroup.Rows[i]["ProgID"].ToString()
                                            select tmpList).Count();

                            if (TmpTotal >= 1)
                            {
                                continue;//已經寫過 則不允許再次寫入
                            }



                            UserInfo.ProgramList.Add(new ProgramModel()
                            {
                                ProgID = BaseGroup.Rows[i]["ProgID"].ToString(),
                                ProgName = BaseGroup.Rows[i]["ProgName"].ToString(),
                                Url = BaseGroup.Rows[i]["Url"].ToString(),
                                ViewLevel = ""
                            });

                        }







                        for (int i = 0; i < dtLoginInfo.Rows.Count;i++ )
                        {

                            //回寫此帳戶可使用之作業權限
                            DataTable dtGroup = DbAccess.ExecuteDataTable("select * from tbProgram Prog LEFT OUTER JOIN tbGroup G ON Prog.Power&G.Power=Prog.Power where G.Power= @Power AND G.GroupID = @GroupID ",
                                new DbParameter[] {
                                                DataAccess.CreateParameter("Power", DbType.String, dtLoginInfo.Rows[i]["Power"].ToString()),
                                                DataAccess.CreateParameter("GroupID", DbType.String,dtLoginInfo.Rows[i]["GroupID"].ToString())
                                }
                            );


                            for (int j = 0; j < dtGroup.Rows.Count; j++)
                            {

                                int TmpTotal = (from tmpList in UserInfo.ProgramList 
                                          where tmpList.ProgID == dtGroup.Rows[j]["ProgID"].ToString()
                                          select tmpList).Count();
                                if (TmpTotal >= 1)
                                {
                                    continue;//已經寫過 則不允許再次寫入
                                }


                                UserInfo.ProgramList.Add(new ProgramModel()
                                {
                                    ProgID = dtGroup.Rows[j]["ProgID"].ToString(),
                                    ProgName = dtGroup.Rows[j]["ProgName"].ToString(),
                                    Url = dtGroup.Rows[j]["Url"].ToString(),
                                    ViewLevel = dtLoginInfo.Rows[i]["ViewLevel"].ToString()
                                });
                            }

                        }



                        /*例外 外出歷程記錄作業  */
                        int TmpOutHistory = (from tmpList in UserInfo.ProgramList
                                        where tmpList.ProgID == "W006"
                                        select tmpList).Count();

                        if (TmpOutHistory == 0)
                        {
                            UserInfo.ProgramList.Add(new ProgramModel()
                            {
                                ProgID = "W006",
                                ProgName = "外出歷程記錄作業",
                                Url ="../Outgoing/History",
                                ViewLevel = ""
                            });
                        }


                    }


                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            catch
            {
                throw new Exception("發生錯誤");
            }

            return UserInfo;
        }











    }
}