using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.POIFS;
using NPOI.Util;
using NPOI.DDF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.Util;
namespace RegalHR.ModelFactory
{
    public class ReportOutgoingModelFactory
    {


        public DataAccess DbAccess = new DataAccess();

        public ReportOutgoingModelFactory()
        {
            //資料庫連線配置
            DbAccess.ConnectionString = RegalLib.DbConnStr;
            DbAccess.ProviderName = RegalLib.ProviderName;
        }



        /// <summary>
        /// 取得報表清單
        /// </summary>
        /// <param name="ProgramList">登入的作業權限</param>
        /// <returns></returns>
        public List<ReportModel> GetReportList(List<ProgramModel> ProgramList)
        {
            //前置碼 
            string PreStr="RG";


            List<ReportModel> ReportList = new List<ReportModel>();

            for (int i = 0; i < ProgramList.Count;i++ )
            {
                if (ProgramList[i].ProgID.IndexOf(PreStr)!=-1)
                {
                    ReportModel tmpReport = new ReportModel();
                    tmpReport.ReportId = ProgramList[i].ProgID;
                    tmpReport.URL = ProgramList[i].Url;
                    tmpReport.ViewLevel = ProgramList[i].ViewLevel;
                    tmpReport.Title = ProgramList[i].ProgName;
                    ReportList.Add(tmpReport);
                }
            }

            return ReportList;
        }







        /// <summary>
        /// 輸出出勤資料Excel
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <returns></returns>
        public Stream RG01ToExcel(ReportFormModel ReportFrom)
        {
            List<RG01Model> RG01List = new List<RG01Model>();

            string SQL = "SELECT *, ";
            SQL +=" CASE Status WHEN 'A' THEN '新增'WHEN 'U' THEN '編輯'WHEN 'D' THEN '刪除' WHEN 'AM' THEN '補登新增' WHEN 'U' THEN '補登編輯' WHEN 'DM' THEN '補登刪除' END AS StatusDisplay, ";
            SQL +=" Convert(char(10),Convert(datetime,OutDate),20) as OutDateFormat,Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,left(UpdateTime,2)+':'+left(right(UpdateTime,4),2) AS UpdateTimeFormat ,left(OutTime,2)+':'+right(OutTime,2) AS OutTimeFormat ,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat  FROM gvOutgoing WHERE 1=1";

            if (ReportFrom.Company!="")
            {
                SQL+=" AND Company = @Company ";
            }

            
            if (ReportFrom.EmpNo!="")
            {
                SQL+=" AND OutMan = @OutMan ";
            }

            
            if (ReportFrom.DepartMentNo!="")
            {
                SQL+=" AND DepartMentNo = @DepartMentNo ";
            }



            SQL += " AND OutDate BETWEEN @StartDate AND @EndDate ";
            SQL += " AND AlermTotal > 0  Order by OutMan,OutDate,OutTime";


            DataTable dt = DbAccess.ExecuteDataTable(SQL,
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, ReportFrom.Company.ToString()),
                    DataAccess.CreateParameter("OutMan", DbType.String, ReportFrom.EmpNo.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, ReportFrom.DepartMentNo.ToString()),
                    DataAccess.CreateParameter("StartDate", DbType.String, ReportFrom.StartDate.ToString().Replace("-","")),
                    DataAccess.CreateParameter("EndDate", DbType.String, ReportFrom.EndDate.ToString().Replace("-",""))
                }
            );


            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                RG01Model tmpM = new RG01Model();
                tmpM.OutId = dt.Rows[i]["OutId"].ToString();
                tmpM.OutDate = dt.Rows[i]["OutDateFormat"].ToString();
                tmpM.EmployeeName = dt.Rows[i]["EmployeeName"].ToString();
                tmpM.OutTime = dt.Rows[i]["OutTimeFormat"].ToString();
                tmpM.OutMan = dt.Rows[i]["OutMan"].ToString();
                tmpM.Status = dt.Rows[i]["StatusDisplay"].ToString();
                tmpM.CompanyName = dt.Rows[i]["CompanyName"].ToString();
                tmpM.DepartMentName = dt.Rows[i]["DepartMentName"].ToString();
                tmpM.AlermTotal = dt.Rows[i]["AlermTotal"].ToString();
                tmpM.GoOutTime = dt.Rows[i]["GoOutTimeFormat"].ToString();
                tmpM.CustomerName = dt.Rows[i]["CustomerName"].ToString();
                tmpM.Location = dt.Rows[i]["Location"].ToString();




                /*
                    SELECT
                    Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,left(UpdateTime,2)+':'+left(right(UpdateTime,4),2) AS UpdateTimeFormat ,left(OutTime,2)+':'+right(OutTime,2) AS OutTimeFormat ,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat
                    FROM gvOutgoing
                */





                SQL = "SELECT *, ";
                SQL +=" CASE Alerm WHEN 0 THEN '正常' WHEN 1 THEN '異常' END AS AlermDisplay,CASE Status WHEN 'A' THEN '新增'WHEN 'U' THEN '編輯'WHEN 'D' THEN '刪除' WHEN 'AM' THEN '補登新增' WHEN 'U' THEN '補登編輯' WHEN 'DM' THEN '補登刪除' END AS StatusDisplay, ";
                SQL +=" Convert(char(10),Convert(datetime,OutDate),20) as OutDateFormat,Convert(char(10),Convert(datetime,UpdateDate),20) as UpdateDateFormat,left(UpdateTime,2)+':'+left(right(UpdateTime,4),2) AS UpdateTimeFormat ,left(OutTime,2)+':'+right(OutTime,2) AS OutTimeFormat ,left(GoOutTime,2)+':'+right(GoOutTime,2) AS GoOutTimeFormat ";
                SQL += " FROM gvOutHistory WHERE OutId = @OutId AND OutMan = @OutMan Order by UpdateDate,UpdateTime";

                DataTable DtDtl = DbAccess.ExecuteDataTable(SQL,
                    new DbParameter[] {
                    DataAccess.CreateParameter("OutId", DbType.String, tmpM.OutId),
                    DataAccess.CreateParameter("OutMan", DbType.String, tmpM.OutMan),
                    }
                );

                for (int j = 0; j < DtDtl.Rows.Count;j++ )
                {
                    RG01DtlModel tmpDtl = new RG01DtlModel();
                    tmpDtl.OutDate = DtDtl.Rows[j]["OutDateFormat"].ToString();
                    tmpDtl.OutTime = DtDtl.Rows[j]["OutTimeFormat"].ToString();
                    tmpDtl.CustomerName = DtDtl.Rows[j]["CustomerName"].ToString();
                    tmpDtl.GoOutTime = DtDtl.Rows[j]["GoOutTimeFormat"].ToString();
                    tmpDtl.RecordManName = DtDtl.Rows[j]["RecordManName"].ToString();
                    tmpDtl.Location = DtDtl.Rows[j]["Location"].ToString();
                    tmpDtl.Status = DtDtl.Rows[j]["StatusDisplay"].ToString();
                    tmpDtl.UpdateDate = DtDtl.Rows[j]["UpdateDateFormat"].ToString();
                    tmpDtl.UpdateTime = DtDtl.Rows[j]["UpdateTimeFormat"].ToString();
                    tmpDtl.Alerm = DtDtl.Rows[j]["AlermDisplay"].ToString();
                    tmpM.RG01Dtl.Add(tmpDtl);
                }



                RG01List.Add(tmpM);

            }






            


            IWorkbook workbook = new XSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = (XSSFSheet)workbook.CreateSheet();




            //定義EmpInfo顏色
            XSSFCellStyle EmpInfoStyle = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFColor BgColor = new XSSFColor();
            BgColor.SetRgb(new byte[] { 102, 204, 255 });
            EmpInfoStyle.SetFillForegroundColor(BgColor);
            EmpInfoStyle.FillPattern = FillPattern.SolidForeground;



            //定義Header顏色
            XSSFCellStyle HeaderStyle = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFColor BgColorHeader = new XSSFColor();
            BgColorHeader.SetRgb(new byte[] { 252, 213, 180 });
            HeaderStyle.SetFillForegroundColor(BgColorHeader);
            HeaderStyle.FillPattern = FillPattern.SolidForeground;



            //定義Header顏色
            XSSFCellStyle BodyStyle = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFColor BgColorBody = new XSSFColor();
            BgColorBody.SetRgb(new byte[] { 216, 218, 188 });
            BodyStyle.SetFillForegroundColor(BgColorBody);
            BodyStyle.FillPattern = FillPattern.SolidForeground;



            int IntRow = 0; //Excel 第幾列
            string IntEmp = "";


            for (int i = 0; i < RG01List.Count; i++)
            {


                if (IntEmp != RG01List[i].OutMan)
                {

                    if (IntEmp!="")
                    {
                        //換行!!
                        IRow SpaceDataRowV = (XSSFRow)sheet.CreateRow(IntRow);
                        XSSFCell tmpCellV = (XSSFCell)SpaceDataRowV.CreateCell(0);
                        IntRow++;
                    }

                    //員工資料＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
                    IRow EmpInfo = (XSSFRow)sheet.CreateRow(IntRow);

                    XSSFCell Cell0 = (XSSFCell)EmpInfo.CreateCell(0);
                    Cell0.SetCellValue("員工編號");
                    Cell0.CellStyle = EmpInfoStyle;

                    XSSFCell Cell1 = (XSSFCell)EmpInfo.CreateCell(1);
                    Cell1.SetCellValue("員工姓名");
                    Cell1.CellStyle = EmpInfoStyle;

                    XSSFCell Cell2 = (XSSFCell)EmpInfo.CreateCell(2);
                    Cell2.SetCellValue("公 司 別");
                    Cell2.CellStyle = EmpInfoStyle;

                    XSSFCell Cell3 = (XSSFCell)EmpInfo.CreateCell(3);
                    Cell3.SetCellValue("部   門");
                    Cell3.CellStyle = EmpInfoStyle;

                    XSSFCell Cell4 = (XSSFCell)EmpInfo.CreateCell(4);
                    Cell4.SetCellValue("異常次數");
                    Cell4.CellStyle = EmpInfoStyle;

                    IntRow++;





                    IRow EmpInfoContent = (XSSFRow)sheet.CreateRow(IntRow);
                    EmpInfoContent.CreateCell(0).SetCellValue(RG01List[i].OutMan);
                    EmpInfoContent.CreateCell(1).SetCellValue(RG01List[i].EmployeeName);
                    EmpInfoContent.CreateCell(2).SetCellValue(RG01List[i].CompanyName);
                    EmpInfoContent.CreateCell(3).SetCellValue(RG01List[i].DepartMentName);



                    int AlermCount = 0;

                     AlermCount = (from tmp in RG01List
                                      where tmp.OutMan == RG01List[i].OutMan
                                      select tmp).Count();


                    EmpInfoContent.CreateCell(4).SetCellValue(AlermCount);
                    IntRow++;





                    IntEmp = RG01List[i].OutMan;
                    //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊員工資料
                }




                //單頭資料＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
                IRow headerRow = (XSSFRow)sheet.CreateRow(IntRow);


                XSSFCell headerCell2 = (XSSFCell)headerRow.CreateCell(2);
                headerCell2.SetCellValue("外出日期");
                headerCell2.CellStyle = HeaderStyle;

                XSSFCell headerCell3 = (XSSFCell)headerRow.CreateCell(3);
                headerCell3.SetCellValue("預計外出時間");
                headerCell3.CellStyle = HeaderStyle;

                XSSFCell headerCell4 = (XSSFCell)headerRow.CreateCell(4);
                headerCell4.SetCellValue("會議時間");
                headerCell4.CellStyle = HeaderStyle;


                XSSFCell headerCell5 = (XSSFCell)headerRow.CreateCell(5);
                headerCell5.SetCellValue("客戶名稱");
                headerCell5.CellStyle = HeaderStyle;

                XSSFCell headerCell6 = (XSSFCell)headerRow.CreateCell(6);
                headerCell6.SetCellValue("地   點");
                headerCell6.CellStyle = HeaderStyle;

                XSSFCell headerCell7 = (XSSFCell)headerRow.CreateCell(7);
                headerCell7.SetCellValue("單據狀態");
                headerCell7.CellStyle = HeaderStyle;
                IntRow++;



                IRow headerContentRow = (XSSFRow)sheet.CreateRow(IntRow);
                headerContentRow.CreateCell(2).SetCellValue(RG01List[i].OutDate);
                headerContentRow.CreateCell(3).SetCellValue(RG01List[i].OutTime);
                headerContentRow.CreateCell(4).SetCellValue(RG01List[i].GoOutTime);
                headerContentRow.CreateCell(5).SetCellValue(RG01List[i].CustomerName);
                headerContentRow.CreateCell(6).SetCellValue(RG01List[i].Location);
                headerContentRow.CreateCell(7).SetCellValue(RG01List[i].Status);
                IntRow++;
                //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊單頭資料





                //單身資料＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
                IRow bodyRow = (XSSFRow)sheet.CreateRow(IntRow);



                XSSFCell bodyCell4 = (XSSFCell)bodyRow.CreateCell(4);
                bodyCell4.SetCellValue("項   次");
                bodyCell4.CellStyle = BodyStyle;

                XSSFCell bodyCell5 = (XSSFCell)bodyRow.CreateCell(5);
                bodyCell5.SetCellValue("系統判定");
                bodyCell5.CellStyle = BodyStyle;

                XSSFCell bodyCell6 = (XSSFCell)bodyRow.CreateCell(6);
                bodyCell6.SetCellValue("單據狀態");
                bodyCell6.CellStyle = BodyStyle;

                XSSFCell bodyCell7 = (XSSFCell)bodyRow.CreateCell(7);
                bodyCell7.SetCellValue("外出日期");
                bodyCell7.CellStyle = BodyStyle;

                XSSFCell bodyCell8 = (XSSFCell)bodyRow.CreateCell(8);
                bodyCell8.SetCellValue("外出時間");
                bodyCell8.CellStyle = BodyStyle;

                XSSFCell bodyCell9 = (XSSFCell)bodyRow.CreateCell(9);
                bodyCell9.SetCellValue("會議時間");
                bodyCell9.CellStyle = BodyStyle;

                XSSFCell bodyCell10 = (XSSFCell)bodyRow.CreateCell(10);
                bodyCell10.SetCellValue("客戶名稱");
                bodyCell10.CellStyle = BodyStyle;

                XSSFCell bodyCell11 = (XSSFCell)bodyRow.CreateCell(11);
                bodyCell11.SetCellValue("地   點");
                bodyCell11.CellStyle = BodyStyle;


                XSSFCell bodyCell12 = (XSSFCell)bodyRow.CreateCell(12);
                bodyCell12.SetCellValue("編輯時間");
                bodyCell12.CellStyle = BodyStyle;


                XSSFCell bodyCell13 = (XSSFCell)bodyRow.CreateCell(13);
                bodyCell13.SetCellValue("編 輯 者");
                bodyCell13.CellStyle = BodyStyle;
                IntRow++;





                for (int j = 0; j < RG01List[i].RG01Dtl.Count;j++ )
                {
                    IRow ItemRow = (XSSFRow)sheet.CreateRow(IntRow);
                    ItemRow.CreateCell(4).SetCellValue((j+1).ToString());
                    ItemRow.CreateCell(5).SetCellValue(RG01List[i].RG01Dtl[j].Alerm);
                    ItemRow.CreateCell(6).SetCellValue(RG01List[i].RG01Dtl[j].Status);
                    ItemRow.CreateCell(7).SetCellValue(RG01List[i].RG01Dtl[j].OutDate);
                    ItemRow.CreateCell(8).SetCellValue(RG01List[i].RG01Dtl[j].OutTime);
                    ItemRow.CreateCell(9).SetCellValue(RG01List[i].RG01Dtl[j].GoOutTime);
                    ItemRow.CreateCell(10).SetCellValue(RG01List[i].RG01Dtl[j].CustomerName);
                    ItemRow.CreateCell(11).SetCellValue(RG01List[i].RG01Dtl[j].Location);
                    ItemRow.CreateCell(12).SetCellValue(RG01List[i].RG01Dtl[j].UpdateDate + " " + RG01List[i].RG01Dtl[j].UpdateTime);
                    ItemRow.CreateCell(13).SetCellValue(RG01List[i].RG01Dtl[j].RecordManName);
                    IntRow++;
                }
                //＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊單身資料







                //換行!!
                IRow SpaceDataRow = (XSSFRow)sheet.CreateRow(IntRow);
                XSSFCell tmpCell = (XSSFCell)SpaceDataRow.CreateCell(0);
                IntRow++;
            }










            workbook.Write(ms);
            ms.Flush();
            //ms.Position = 0;

            sheet = null;
            workbook = null;

            return ms;
        }



    }
}