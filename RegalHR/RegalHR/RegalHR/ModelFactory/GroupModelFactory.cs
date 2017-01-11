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
    public class GroupModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public GroupModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }




        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<GroupModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;




            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvGroup WHERE 1 = 1 " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;


            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GroupModel M = new GroupModel();
                M.GroupID = dt.Rows[i]["GroupID"].ToString();
                M.GroupName = dt.Rows[i]["GroupName"].ToString();
                M.Power = Convert.ToInt32(dt.Rows[i]["Power"].ToString());
                M.ViewLevel = dt.Rows[i]["ViewLevel"].ToString();
                M.ViewLevelName = dt.Rows[i]["ViewLevelName"].ToString();
                DataList.Add(M);
            }

            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());

        }








        /// <summary>
        /// 新增Group資料
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        public bool GroupAdd(GroupModel Group)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();

            Group.Power = (from p in Group.GroupProgramList
                           where p.ChkFlag == "1"
                           select p.Power).Sum();


            try
            {
                DbAccess.ExecuteNonQuery("INSERT INTO tbGroup(GroupID,GroupName,Power,ViewLevel) VALUES (@GroupID,@GroupName,@Power,@ViewLevel) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("GroupID", DbType.String, Group.GroupID.ToString()),
                    DataAccess.CreateParameter("GroupName", DbType.String, Group.GroupName.ToString()),
                    DataAccess.CreateParameter("Power", DbType.String, Group.Power.ToString()),
                     DataAccess.CreateParameter("ViewLevel", DbType.String, Group.ViewLevel.ToString()),


                    });

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











        /// <summary>
        /// 編輯Group資料
        /// </summary>
        /// <param name="Group">Group</param>
        /// <returns></returns>
        public bool GroupEdit(GroupModel Group)
        {



            DbTransaction objTrans = DbAccess.CreateDbTransaction();



            Group.Power = (from p in Group.GroupProgramList
                           where p.ChkFlag == "1"
                           select p.Power).Sum();
                              




            try
            {
                DbAccess.ExecuteNonQuery("UPDATE tbGroup SET GroupID=@GroupID,GroupName=@GroupName,Power=@Power,ViewLevel=@ViewLevel WHERE GroupID = @EditPK ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EditPK", DbType.String, Group.EditPK.ToString()),
                    DataAccess.CreateParameter("GroupID", DbType.String, Group.GroupID.ToString()),
                    DataAccess.CreateParameter("GroupName", DbType.String, Group.GroupName.ToString()),
                    DataAccess.CreateParameter("Power", DbType.String, Group.Power.ToString()),
                    DataAccess.CreateParameter("ViewLevel", DbType.String, Group.ViewLevel.ToString()),

                    });

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







        /// <summary>
        /// 刪除群組
        /// </summary>
        /// <param name="GroupID">GroupID</param>
        /// <returns>bool</returns>
        public bool GroupDelete(string GroupID)
        {



            DataTable dt = DbAccess.ExecuteDataTable("SELECT Count(*) as Total FROM tbEmpGroup Where GroupID = @GroupID ",
                new DbParameter[] {
                                DataAccess.CreateParameter("GroupID", DbType.String, GroupID.ToString())
                            }
            );

            if (dt.Rows[0]["Total"].ToString() != "0")
            {
                throw new Exception("權限群組尚有人員!");
            }


            DbTransaction objTrans = DbAccess.CreateDbTransaction();
            try
            {








                DbAccess.ExecuteNonQuery("DELETE tbGroup WHERE GroupID = @GroupID ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("GroupID", DbType.String, GroupID.ToString()),
                    });

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




        /// <summary>
        /// 取得該Group資料
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public GroupModel GetGroupData(string GroupID)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbGroup Where GroupID = @GroupID ",
                new DbParameter[] {
                    DataAccess.CreateParameter("GroupID", DbType.String, GroupID.ToString())
                }
            );

            GroupModel Group = null;


            if (dt.Rows.Count == 0)
            {
                throw new Exception("查無資料");
            }
            else if (dt.Rows.Count >= 2)
            {
                throw new Exception("有多筆資料");
            }
            else
            {
                Group = new GroupModel();

                Group.GroupID = dt.Rows[0]["GroupID"].ToString();
                Group.GroupName = dt.Rows[0]["GroupName"].ToString();
                Group.Power = int.Parse(dt.Rows[0]["Power"].ToString());
                Group.ViewLevel = dt.Rows[0]["ViewLevel"].ToString();

                
                Group.EditPK = Group.GroupID;//修改專用PK值





                DataTable dt2 = DbAccess.ExecuteDataTable("select M.*,CASE WHEN M.ProgID=D.ProgID THEN '1' ELSE '0' END AS ChkFlag from  tbProgram M LEFT OUTER JOIN (SELECT Prog.* from tbProgram Prog LEFT OUTER JOIN tbGroup  G ON Prog.Power & G.Power=Prog .Power where G.Power= @Power AND G.GroupID = @GroupID) D ON M.ProgID=D.ProgID WHERE M.FlagType='0' ORDER BY  M.ProgID",
                        new DbParameter[] {
                                        DataAccess.CreateParameter("Power", DbType.String, Group.Power.ToString()),
                                        DataAccess.CreateParameter("GroupID", DbType.String, Group.GroupID.ToString())
                        }
                    );


                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    Group.GroupProgramList.Add(new ProgramModel()
                    {
                        ProgID = dt2.Rows[i]["ProgID"].ToString(),
                        ProgName = dt2.Rows[i]["ProgName"].ToString(),
                        Power = int.Parse(dt2.Rows[i]["Power"].ToString()),
                        ChkFlag = dt2.Rows[i]["ChkFlag"].ToString()
                    });
                }


            }

            return Group;
        }


    }
}