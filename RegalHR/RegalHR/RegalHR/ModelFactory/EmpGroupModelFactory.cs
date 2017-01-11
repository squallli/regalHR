using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;

namespace RegalHR.ModelFactory
{
    public class EmpGroupModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public EmpGroupModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }

        

        public bool EmpGroupSave(EmpGroupModel EmpGroup)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {


                DbAccess.ExecuteNonQuery("DELETE FROM tbEmpGroup Where EmployeeNo = @EmployeeNo ", objTrans,
                      new DbParameter[] {
                      DataAccess.CreateParameter("EmployeeNo", DbType.String, EmpGroup.Emp.EmployeeNo.ToString())
                      }
                );



                for (int i = 0; i < EmpGroup.GroupList.Count(); i++)
                {
                    DbAccess.ExecuteNonQuery("INSERT INTO tbEmpGroup (EmployeeNo,GroupID) VALUES (@EmployeeNo,@GroupID) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String,  EmpGroup.Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("GroupID", DbType.String,  EmpGroup.GroupList[i].GroupID.ToString())
                    });
                }


                objTrans.Commit();       

                return true;
            }
            catch
            {
                objTrans.Rollback();
                return false;
            }
            finally
            {
                if (objTrans != null)
                {
                    objTrans.Dispose();
                }
            }

        }






        public EmpGroupModel GetEmpGroup(string EmpNo)
        {


            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvEmployeeAll Where EmployeeNo = @EmployeeNo ",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, EmpNo.ToString())
                }
            );

            EmpModel Emp = null;
            Emp = new EmpModel();
            Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
            Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
            Emp.DepartMentName = dt.Rows[0]["DepartMentName"].ToString();
            Emp.DepartMentNo = dt.Rows[0]["DepartMentNo"].ToString();
            Emp.Company = dt.Rows[0]["Company"].ToString();
            Emp.CompanyName = dt.Rows[0]["CompanyName"].ToString();

            Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();
            //########################





            EmpGroupModel EmpGroup = new EmpGroupModel();
            EmpGroup.Emp = Emp;



            string sql ="";



            sql = "SELECT GroupID,GroupName,ViewLevel,Power,0 as Flag FROM tbGroup WHERE GroupID Not IN (SELECT GroupID FROM tbEmpGroup WHERE EmployeeNo=@EmployeeNo) "
                + " UNION SELECT GP.GroupID,GP.GroupName,GP.ViewLevel,GP.Power,1 as Flag FROM tbEmpGroup EG LEFT OUTER JOIN tbGroup GP ON GP.GroupID=EG.GroupID WHERE EmployeeNo=@EmployeeNo ";




            dt = DbAccess.ExecuteDataTable(sql,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, EmpNo)
                    });


            List<ItemModel> ViewLevelList = GeneralModelFactory.GetGroupViewLevelList();

            ProgramModelFactory Factory = new ProgramModelFactory();
            List<ProgramModel> ProgramList = Factory.GetGroupProgramList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tmp = (from tmpViewLevel in ViewLevelList
                              where tmpViewLevel.ItemID == dt.Rows[i]["ViewLevel"].ToString()
                              select tmpViewLevel.ItemName).First();


                int tmpPower= int.Parse(dt.Rows[i]["Power"].ToString());

                List<ProgramModel> tmpProgramList = (from tmpProgram in ProgramList
                                      where (tmpPower&tmpProgram.Power) == tmpProgram.Power
                                      select tmpProgram).ToList();
 

                if(dt.Rows[i]["Flag"].ToString()=="1")
                {
                    EmpGroup.GroupList.Add(new GroupModel()
                    {
                        GroupID = dt.Rows[i]["GroupID"].ToString(),
                        GroupName = dt.Rows[i]["GroupName"].ToString(),
                        ViewLevel = tmp,
                        GroupProgramList = tmpProgramList
                    });
                }
                else
                {
                    EmpGroup.NoGroupList.Add(new GroupModel()
                    {
                        GroupID = dt.Rows[i]["GroupID"].ToString(),
                        GroupName = dt.Rows[i]["GroupName"].ToString(),
                        ViewLevel = tmp,
                        GroupProgramList = tmpProgramList
                    });
                }


            }
  



            return EmpGroup;
        }



        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<EmpModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;



            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvEmployeeAll WHERE 1 = 1 AND Status='1' " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            string EmpNoStr = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpModel M = new EmpModel();
                M.Company = dt.Rows[i]["Company"].ToString();
                M.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                M.EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString();
                M.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                M.EmployeeEName = dt.Rows[i]["EmployeeEName"].ToString();
                M.DepartMentName = dt.Rows[i]["DepartMentName"].ToString();
                M.DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString();
                M.CardNo = dt.Rows[i]["CardNo"].ToString();

                EmpNoStr += "'" + M.EmployeeNo + "',";
                DataList.Add(M);
            }





            if (dt.Rows.Count != 0)
            {


                EmpNoStr = EmpNoStr.TrimEnd(',');
                Sql = "SELECT EmployeeNo,GroupName,ViewLevel,ViewLevelName FROM tbEmpGroup EG LEFT OUTER JOIN gvGroup GP ON EG.GroupID=GP.GroupID  WHERE EmployeeNo IN ( " + EmpNoStr + " ) ";

                DataTable dt2 = DbAccess.ExecuteDataTable(Sql);

                for (int i = 0; i < DataList.Count; i++)
                {
                    DataRow[] DR = dt2.Select(" EmployeeNo = '" + DataList[i].EmployeeNo + "' ");


                    for (int j = 0; j < DR.Count(); j++)
                    {
                        GroupModel Tmp = new GroupModel();
                        Tmp.GroupName = DR[j]["GroupName"].ToString();

                        Tmp.ViewLevelName = DR[j]["ViewLevelName"].ToString();
                        Tmp.ViewLevel = DR[j]["ViewLevel"].ToString();


                        DataList[i].Group.Add(Tmp);
                    }
                }

            }





            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }




        /// <summary>
        /// 輸入部門編號帶出部門底下之員工
        /// </summary>
        /// <param name="DepartMentNo">部門編號</param>
        /// <returns></returns>
        public List<EmpModel> GetEmpList(string DepartMentNo)
        {


            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbEmployee Where DepartMentNo=@DepartMentNo AND Status='1'  ORDER BY DepartMentNo ", new DbParameter[] {
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, DepartMentNo.ToString())
                }
            );


            List<EmpModel> EmpList = new List<EmpModel>();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EmpList.Add(new EmpModel()
                {
                    EmployeeNo = dt.Rows[i]["EmployeeNo"].ToString(),
                    EmployeeName = dt.Rows[i]["EmployeeName"].ToString()
                });

            }

            return EmpList;
        }


    }









}