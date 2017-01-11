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
    public class TempEmpModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public TempEmpModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }




        /// <summary>
        /// 新增TempEmp資料
        /// </summary>
        /// <param name="Emp">EmpModel</param>
        /// <returns></returns>
        public bool TempEmpAdd(EmpModel Emp)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            DataTable ChkCard = DbAccess.ExecuteDataTable("SELECT Count(*) AS Total FROM tbCard WHERE CardNo = @CardNo AND  Status='1' ",
                    new DbParameter[] {
                     DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString())
                     }
            );

            if ( ChkCard.Rows[0]["Total"].ToString()!="0")
            {
                throw new Exception("卡號重覆!");
            }



            try
            {
                DbAccess.ExecuteNonQuery("INSERT INTO tbTempEmployee(EmployeeNo,EmployeeName,DepartMentNo,EmployeeEName,Status,Company) VALUES (@EmployeeNo,@EmployeeName,@DepartMentNo,@EmployeeEName,@Status,@Company) ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("EmployeeName", DbType.String, Emp.EmployeeName.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, Emp.DepartMentNo.ToString()),
                    DataAccess.CreateParameter("EmployeeEName", DbType.String, Emp.EmployeeEName.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String, "1"),
                    DataAccess.CreateParameter("Company", DbType.String, Emp.Company.ToString()),
                    });



                if (Emp.CardNo != "")
                {
                    DbAccess.ExecuteNonQuery("INSERT INTO tbCard(EmployeeNo,CardNo,Status,UpdateDate,EffectiveDate,ExpiryDate) VALUES (@EmployeeNo,@CardNo,@Status,@UpdateDate,@EffectiveDate,@ExpiryDate) ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                        DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
                        DataAccess.CreateParameter("Status", DbType.String, "1"),
                        DataAccess.CreateParameter("EffectiveDate", DbType.String, Emp.CardEffectiveDate.ToString()),
                        DataAccess.CreateParameter("ExpiryDate", DbType.String, Emp.CardExpiryDate.ToString()),
                        DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
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







        /// <summary>
        /// 取得新增員工編號
        /// </summary>
        /// <returns>新增員工編號</returns>
        public string GetNewTempEmpNo()
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT ISNULL(MAX(Replace(EmployeeNo,'T','')),'00000') AS NewTempEmpNo FROM tbTempEmployee");


            int NewEmpNo = 0 ;
            
            NewEmpNo = int.Parse(dt.Rows[0]["NewTempEmpNo"].ToString());
            NewEmpNo++;

            return "T"+NewEmpNo.ToString("00000");
        }




        /// <summary>
        /// 編輯Emp資料
        /// </summary>
        /// <param name="Program">ProgramModel</param>
        /// <returns></returns>
        public bool TempEmpEdit(EmpModel Emp)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {


                DbAccess.ExecuteNonQuery("UPDATE tbTempEmployee SET EmployeeName=@EmployeeName,DepartMentNo=@DepartMentNo,EmployeeEName=@EmployeeEName,Company=@Company WHERE EmployeeNo = @EditPK ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EditPK", DbType.String, Emp.EditPK.ToString()),
                    DataAccess.CreateParameter("EmployeeName", DbType.String, Emp.EmployeeName.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, Emp.DepartMentNo.ToString()),
                    DataAccess.CreateParameter("EmployeeEName", DbType.String, Emp.EmployeeEName.ToString()),
                     DataAccess.CreateParameter("Company", DbType.String, Emp.Company.ToString()),
                    });



                //當此員工有卡號時 就得更新..卡號有效日
                if(Emp.CardStatus=="1")
                {
                    DbAccess.ExecuteNonQuery("UPDATE tbCard SET EffectiveDate=@EffectiveDate,ExpiryDate=@ExpiryDate WHERE EmployeeNo = @EditPK AND CardNo = @CardNo AND Status = '1' ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("EditPK", DbType.String, Emp.EditPK.ToString()),
                        DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
                        DataAccess.CreateParameter("EffectiveDate", DbType.String, Emp.CardEffectiveDate.ToString()),
                        DataAccess.CreateParameter("ExpiryDate", DbType.String, Emp.CardExpiryDate.ToString()),
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







        /// <summary>
        /// 註銷該暫時員工
        /// </summary>
        /// <param name="ProgID">ProgID</param>
        /// <returns>bool</returns>
        public bool TempEmpDelete(string TempEmpID)
        {
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {

                //將卡片註銷
                DbAccess.ExecuteNonQuery("UPDATE tbCard SET Status='0',UpdateDate=@UpdateDate WHERE EmployeeNo = @EmployeeNo AND Status = '1' ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, TempEmpID),
                    });



                //將該使用者權限刪除
                DbAccess.ExecuteNonQuery("DELETE FROM tbEmpGroup WHERE EmployeeNo = @EmployeeNo ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, TempEmpID),
                    });



                //註銷暫時員工!
                DbAccess.ExecuteNonQuery("UPDATE tbTempEmployee SET Status='0' WHERE EmployeeNo=@EmployeeNo", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, TempEmpID),
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






        public void GetGridViewList(string Where, string Orderby, int PageNum, int Limit, ref List<EmpModel> DataList, ref int TotalRecord)
        {

            int offset = PageNum * Limit;



            //主要SQL
            string SqlMain = "SELECT ROW_NUMBER() OVER (ORDER BY " + Orderby + ") AS RowNum,* FROM gvTempEmployee WHERE 1 = 1 " + Where;

            string Sql = "SELECT TOP " + Limit + " * FROM ( ";
            Sql += SqlMain;
            Sql += " ) TEMP WHERE RowNum> " + offset;



           DataTable dt = DbAccess.ExecuteDataTable(Sql);



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
                DataList.Add(M);
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
        public List<EmpModel> GetTempEmpList(string DepartMentNo)
        {
            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbTempEmployee Where DepartMentNo=@DepartMentNo AND Status='1'  ORDER BY DepartMentNo ", new DbParameter[] {
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

        public List<DepartMentModel> GetDepartMentList()
        {
            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM tbDepartMent ORDER BY DepartMentNo ");

            List<DepartMentModel> DepartMentList = new List<DepartMentModel>();


            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                DepartMentList.Add(new DepartMentModel()
                {
                    DepartMentNo = dt.Rows[i]["DepartMentNo"].ToString(),
                    DepartMentName = dt.Rows[i]["DepartMentName"].ToString()
                });

            }

            return DepartMentList;
            
        }




        /// <summary>
        /// 取得臨時員工資料
        /// </summary>
        /// <param name="EmployeeNo"></param>
        /// <returns></returns>
        public EmpModel GetTempEmpData(string EmployeeNo)
        {

            DataTable dt = DbAccess.ExecuteDataTable("SELECT * FROM gvTempEmployee Where EmployeeNo = @EmployeeNo ",
                new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, EmployeeNo.ToString())
                }
            );

            EmpModel Emp = null;


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
                Emp = new EmpModel();

                Emp.EmployeeNo = dt.Rows[0]["EmployeeNo"].ToString();
                Emp.EmployeeName = dt.Rows[0]["EmployeeName"].ToString();
                Emp.DepartMentName = dt.Rows[0]["DepartMentName"].ToString();
                Emp.DepartMentNo = dt.Rows[0]["DepartMentNo"].ToString();

                Emp.EmployeeEName = dt.Rows[0]["EmployeeEName"].ToString();
                Emp.Company = dt.Rows[0]["Company"].ToString();
                Emp.CompanyName = dt.Rows[0]["CompanyName"].ToString();




                Emp.EditPK = Emp.EmployeeNo;//修改專用PK值







                DataTable dt2 = DbAccess.ExecuteDataTable("SELECT CardNo,Convert(char(10),Convert(datetime,EffectiveDate),120) AS EffectiveDateFormat,Convert(char(10),Convert(datetime,ExpiryDate),120) AS ExpiryDateFormat FROM tbCard WHERE EmployeeNo = @EmployeeNo AND  Status='1' ",
                    new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString())
                    }
                );


                if( dt2.Rows.Count==0 )
                {
                    Emp.CardStatus = "0"; 
                    Emp.CardNo = null; 
                }
                else if (dt2.Rows.Count == 1)
                {
                    Emp.CardStatus = "1"; 
                    Emp.CardNo = dt2.Rows[0]["CardNo"].ToString();
                    Emp.CardEffectiveDate = dt2.Rows[0]["EffectiveDateFormat"].ToString();
                    Emp.CardExpiryDate = dt2.Rows[0]["ExpiryDateFormat"].ToString();
                }
                else
                {
                    throw new Exception("有多筆卡號");
                }
            }

            return Emp;
        }



        public bool InsertCardNo(EmpModel Emp,string NewCardNo)
        {


            DataTable ChkDt = DbAccess.ExecuteDataTable("SELECT Count(*) AS TOTAL FROM tbCard WHERE CardNo = @CardNo AND Status = '1' ",
                new DbParameter[] {
                                    DataAccess.CreateParameter("CardNo", DbType.String, NewCardNo)
                                }
            );

            if (ChkDt.Rows[0]["TOTAL"].ToString() != "0")
            {
                throw new Exception("重覆卡號!");
            }



            DbTransaction objTrans = DbAccess.CreateDbTransaction();

            try
            {
                //檢查CardNo是否有重覆&使用中









                DbAccess.ExecuteNonQuery("INSERT INTO tbCard(EmployeeNo,CardNo,Status,UpdateDate,EffectiveDate,ExpiryDate) VALUES(@EmployeeNo,@CardNo,@Status,@UpdateDate,@EffectiveDate,@ExpiryDate)", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("CardNo", DbType.String, NewCardNo),
                    DataAccess.CreateParameter("Status", DbType.String, "1"),
                    DataAccess.CreateParameter("EffectiveDate", DbType.String, DateTime.Now.ToString("yyyyMMdd")),
                    DataAccess.CreateParameter("ExpiryDate", DbType.String, "99991231"),
                    DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
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
        /// 臨時員工轉正職
        /// </summary>
        /// <param name="TurnEmpNo">正式員工編號</param>
        /// <param name="Emp">臨時員工</param>
        /// <returns></returns>
        public bool TurnFullTime(string TurnEmpNo, EmpModel Emp)
        {
            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {

                //將轉正式員工旗標 2 改為 1
                DbAccess.ExecuteNonQuery("UPDATE tbEmployee SET Status=@Status WHERE EmployeeNo=@EmployeeNo", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, TurnEmpNo),
                    DataAccess.CreateParameter("Status", DbType.String, "1"),
                    });


                //註銷臨時員工
                DbAccess.ExecuteNonQuery("UPDATE tbTempEmployee SET FullTimeEmployeeNo=@FullTimeEmployeeNo, Status=@Status WHERE EmployeeNo=@EmployeeNo", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("FullTimeEmployeeNo", DbType.String, TurnEmpNo),
                    DataAccess.CreateParameter("Status", DbType.String, "0"),
                    });




                //將該使用者權限刪除
                DbAccess.ExecuteNonQuery("DELETE FROM tbEmpGroup WHERE EmployeeNo = @EmployeeNo ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    });



                //卡號部分
                if(Emp.CardStatus=="1")
                {
                    //臨時員工卡號註銷
                    DbAccess.ExecuteNonQuery("UPDATE tbCard SET Status='0',UpdateDate=@UpdateDate WHERE EmployeeNo = @EmployeeNo AND Status = '1' ", objTrans,
                        new DbParameter[] {
                        DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                        });






                    //若是該正式員工無卡號 則將原本暫時員工卡號回寫回去!
                    DataTable ChkDt = DbAccess.ExecuteDataTable("SELECT CardNo FROM tbCard WHERE EmployeeNo=@EmployeeNo AND Status='1' ",
                            new DbParameter[] {
                            DataAccess.CreateParameter("EmployeeNo", DbType.String, TurnEmpNo)
                            }
                        );

                    if (ChkDt.Rows.Count==0)
                    {

                        DbAccess.ExecuteNonQuery("INSERT INTO tbCard(EmployeeNo,CardNo,Status,UpdateDate,EffectiveDate,ExpiryDate) VALUES(@EmployeeNo,@CardNo,@Status,@UpdateDate,@EffectiveDate,@ExpiryDate)", objTrans,
                            new DbParameter[] {
                            DataAccess.CreateParameter("EmployeeNo", DbType.String, TurnEmpNo),
                            DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
                            DataAccess.CreateParameter("Status", DbType.String, "1"),
                            DataAccess.CreateParameter("EffectiveDate", DbType.String,Emp.CardEffectiveDate.ToString()),
                            DataAccess.CreateParameter("ExpiryDate", DbType.String, Emp.CardExpiryDate.ToString()),
                            DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                            });
                    }
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


        /// <summary>
        /// 註銷卡號
        /// </summary>
        /// <param name="EmployeeNo"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        public bool DeleteCardNo(EmpModel Emp)
        {

            DbTransaction objTrans = DbAccess.CreateDbTransaction();


            try
            {
                DbAccess.ExecuteNonQuery("UPDATE tbCard SET Status='0',UpdateDate=@UpdateDate WHERE EmployeeNo = @EmployeeNo AND CardNo=@CardNo AND Status = '1' ", objTrans,
                    new DbParameter[] {
                    DataAccess.CreateParameter("UpdateDate", DbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, Emp.EmployeeNo.ToString()),
                    DataAccess.CreateParameter("CardNo", DbType.String, Emp.CardNo.ToString()),
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
        /// 取得需要轉正職之員工
        /// </summary>
        /// <returns></returns>
        public List<EmpModel> GetTurnEmpList(EmpModel Emp)
        {
            DataTable dt = DbAccess.ExecuteDataTable("SELECT EmployeeNo,EmployeeName FROM tbEmployee WHERE Status='2' AND  (EmployeeName=@EmployeeName OR EmployeeEName=@EmployeeEName)  Order by EmployeeNo DESC",
                    new DbParameter[] {
                        DataAccess.CreateParameter("EmployeeName", DbType.String, Emp.EmployeeName.ToString()),
                        DataAccess.CreateParameter("Company", DbType.String, Emp.Company.ToString()),
                        DataAccess.CreateParameter("EmployeeEName", DbType.String, Emp.EmployeeEName.ToString()),
                    });
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