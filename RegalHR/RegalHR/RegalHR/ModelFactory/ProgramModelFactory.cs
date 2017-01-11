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
    public class ProgramModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public ProgramModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }




        /// <summary>
        /// 新增Program資料
        /// </summary>
        /// <param name="Program">ProgramModel</param>
        /// <returns></returns>
        public bool ProgramAdd(ProgramModel Program)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {
                DbAccess.ExecuteNonQuery("INSERT INTO tbProgram(ProgID,ProgName,Power,Url,FlagType) VALUES (@ProgID,@ProgName,@Power,@Url,@FlagType) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("ProgID", DbType.String, Program.ProgID.ToString()),
                    DataAccess.CreateParameter("ProgName", DbType.String, Program.ProgName.ToString()),
                    DataAccess.CreateParameter("Power", DbType.Int64, Program.Power.ToString()),
                    DataAccess.CreateParameter("Url", DbType.String, Program.Url.ToString()),
                    DataAccess.CreateParameter("FlagType", DbType.String, Program.FlagType.ToString())
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
        /// 編輯Program資料
        /// </summary>
        /// <param name="Program">ProgramModel</param>
        /// <returns></returns>
        public bool ProgramEdit(ProgramModel Program)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {
                DbAccess.ExecuteNonQuery("UPDATE tbProgram SET ProgID=@ProgID,ProgName=@ProgName,Power=@Power,Url=@Url,FlagType=@FlagType WHERE ProgID = @EditPK ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EditPK", DbType.String, Program.EditPK.ToString()),
                    DataAccess.CreateParameter("ProgID", DbType.String, Program.ProgID.ToString()),
                    DataAccess.CreateParameter("ProgName", DbType.String, Program.ProgName.ToString()),
                    DataAccess.CreateParameter("Power", DbType.Int64, Program.Power.ToString()),
                    DataAccess.CreateParameter("Url", DbType.String, Program.Url.ToString()),
                    DataAccess.CreateParameter("FlagType", DbType.String, Program.FlagType.ToString())
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
        /// 刪除Program
        /// </summary>
        /// <param name="ProgID">ProgID</param>
        /// <returns>bool</returns>
        public bool ProgramDelete(string ProgID)
        {
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {
                DbAccess.ExecuteNonQuery("DELETE tbProgram WHERE ProgID = @ProgID ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("ProgID", DbType.String, ProgID.ToString()),
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






        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<ProgramModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;



            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvProgram WHERE 1 = 1 " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



            DataTable dt = DbAccess.ExecuteDataTable(Sql);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProgramModel M = new ProgramModel();
                M.ProgID = dt.Rows[i]["ProgID"].ToString();
                M.ProgName = dt.Rows[i]["ProgName"].ToString();
                M.Power = Convert.ToInt32(dt.Rows[i]["Power"].ToString());
                DataList.Add(M);
            }





            //取得總數量
            Sql = "SELECT COUNT(*) AS TOTAL FROM (" + SqlMain + ") TEMP";
            dt = DbAccess.ExecuteDataTable(Sql);
            TotalRecord = Convert.ToInt32(dt.Rows[0]["TOTAL"].ToString());



        }




        public ProgramModel GetProgramData(string ProgID)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbProgram Where ProgID = @ProgID ",
                new DbParameter[] {
                    DataAccess.CreateParameter("ProgID", DbType.String, ProgID.ToString())
                }
            );

            ProgramModel Program = null;


            if(dt.Rows.Count==0)
            {
                throw new Exception("查無資料");
            }
            else if(dt.Rows.Count>=2)
            {
                throw new Exception("有多筆資料");
            }
            else
            {
                Program=new ProgramModel();

                Program.ProgID = dt.Rows[0]["ProgID"].ToString();
                Program.ProgName = dt.Rows[0]["ProgName"].ToString();
                Program.Power = int.Parse(dt.Rows[0]["Power"].ToString());
                Program.Url = dt.Rows[0]["Url"].ToString();
                Program.FlagType = dt.Rows[0]["FlagType"].ToString();
                Program.EditPK = Program.ProgID.ToString();//修改專用PK值
            }

            return Program;
        }





        /// <summary>
        /// 列出Program所有資料
        /// </summary>
        /// <returns></returns>
        public List<ProgramModel> GetProgramList()
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbProgram Order by ProgID ");

            List<ProgramModel> ProgramList = new List<ProgramModel>();


            for(int i=0;i<dt.Rows.Count;i++)
            {
                ProgramList.Add(new ProgramModel(){
                    
                    ProgID = dt.Rows[i]["ProgID"].ToString(),
                    ProgName = dt.Rows[i]["ProgName"].ToString(),
                    Power = int.Parse(dt.Rows[i]["Power"].ToString()),
                    Url = dt.Rows[i]["Url"].ToString(),
                    ChkFlag= "0"
                });

            }

            return ProgramList;
        }






        /// <summary>
        /// 列出群組要用Program所有資料
        /// </summary>
        /// <returns></returns>
        public List<ProgramModel> GetGroupProgramList()
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbProgram WHERE FlagType='0' Order by ProgID ");

            List<ProgramModel> ProgramList = new List<ProgramModel>();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProgramList.Add(new ProgramModel()
                {

                    ProgID = dt.Rows[i]["ProgID"].ToString(),
                    ProgName = dt.Rows[i]["ProgName"].ToString(),
                    Power = int.Parse(dt.Rows[i]["Power"].ToString()),
                    Url = dt.Rows[i]["Url"].ToString(),
                    ChkFlag = "0"
                });

            }

            return ProgramList;
        }
        
    }
}