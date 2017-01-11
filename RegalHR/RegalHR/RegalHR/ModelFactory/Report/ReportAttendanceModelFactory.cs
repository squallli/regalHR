using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RegalHRModel;
using REGAL.Data.DataAccess;
using System.Data;
using System.Data.Common;
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
    public class ReportAttendanceModelFactory
    {
        public DataAccess DbAccess = new DataAccess();

        public ReportAttendanceModelFactory()
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
            string PreStr="RA";


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
        /// 輸出員工出勤記錄表Excel
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <returns></returns>
        public static Stream RA01ToExcel(DataTable SourceTable)
        {
            IWorkbook workbook = new XSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = (XSSFSheet)workbook.CreateSheet();
            IRow headerRow = (XSSFRow)sheet.CreateRow(0);


            //定義超過五分鐘後的顏色
            XSSFCellStyle LateStyle = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFFont font = workbook.CreateFont() as XSSFFont;
            font.Color = IndexedColors.DarkRed.Index;
            LateStyle.SetFont(font);

            //定義超過遲到 五分鐘內的顏色
            XSSFCellStyle LateStyle5Min = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFFont font2 = workbook.CreateFont() as XSSFFont;
            font2.Color = IndexedColors.Green.Index;
            LateStyle5Min.SetFont(font2);

            //定義曠職顏色
            XSSFCellStyle AbsStyle = (XSSFCellStyle)workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFColor BgColor = new XSSFColor();
            BgColor.SetRgb(new byte[] { 255, 255, 0 });
            AbsStyle.SetFillForegroundColor(BgColor);
            AbsStyle.FillPattern = FillPattern.SolidForeground;
            AbsStyle.SetFillForegroundColor(BgColor);

            //定義輸出欄位
            List<DataTableDisplay> DisplayList = new List<DataTableDisplay>();
            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeNo",
                DisplayName = "員工編號"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "DepartMentName",
                DisplayName = "部門"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeName",
                DisplayName = "姓名"
            });

            //2016.11.12 Scott新增英文名欄位
            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeEName",
                DisplayName = "英文名"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CompanyName",
                DisplayName = "公司別"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "WorkDay",
                DisplayName = "日期"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "StartWorkTime",
                DisplayName = "上班"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EndWorkTime",
                DisplayName = "下班"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "StartWorkOvertime",
                DisplayName = "加班上班"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EndWorkOvertime",
                DisplayName = "加班下班"
            });
            // 1051129 Scott 將出勤描述 分為上午、下午、加班 3欄
            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "AttendanceDescM",
                DisplayName = "出勤描述(上午)"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "AttendanceDescN",
                DisplayName = "出勤描述(下午)"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "AttendanceDescOT",
                DisplayName = "出勤描述(加班)"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "LateMinFormat",
                DisplayName = "遲到"
            });
            // Scott 105.12.27 移除備註欄為
            //DisplayList.Add(new DataTableDisplay()
            //{
            //    ColumnName = "Memo",
            //    DisplayName = "備註"
            //});

            for (int i = 0; i < DisplayList.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(DisplayList[i].DisplayName);
            }

            int IntRow = 1;
            if (SourceTable.Rows.Count > 0)
            {
                string BrEmpNo = SourceTable.Rows[0]["EmployeeNo"].ToString();
                string Test = "";
                for (int i = 0; i < SourceTable.Rows.Count; i++)
                {

                    if (BrEmpNo != SourceTable.Rows[i]["EmployeeNo"].ToString())
                    {
                        //換行!!
                        IRow tmpDataRow = (XSSFRow)sheet.CreateRow(IntRow);
                        XSSFCell tmpCell = (XSSFCell)tmpDataRow.CreateCell(0);
                        IntRow++;
                        BrEmpNo = SourceTable.Rows[i]["EmployeeNo"].ToString();
                    }

                    IRow dataRow = (XSSFRow)sheet.CreateRow(IntRow);


                    for (int j = 0; j < DisplayList.Count; j++)
                    {

                        try
                        {
                            Test = SourceTable.Rows[i][DisplayList[j].ColumnName].ToString();
                        }
                        catch
                        {
                            //查無此欄位 則空著!!
                            XSSFCell CellR = (XSSFCell)dataRow.CreateCell(j);
                            CellR.SetCellValue("");
                            continue;
                        }

                        //** 人事部稽核用與Web輸出不同!!! **
                        //若是正常打卡且有出差,則出差兩字拿掉
                        /*
                        if (DisplayList[j].ColumnName == "AttendanceDescM" && SourceTable.Rows[i]["AttendanceDescM"].ToString().IndexOf("出差") != -1)
                        {

                            if ((SourceTable.Rows[i]["StartWorkTime"].ToString() != "" && SourceTable.Rows[i]["EndWorkTime"].ToString() != ""))
                            {
                                //正常上班  正常下班
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace("出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace("出差", "");
                            }
                            else if ((SourceTable.Rows[i]["StartWorkTime"].ToString() != "" && SourceTable.Rows[i]["EndWorkOvertime"].ToString() != ""))
                            {
                                //正常上班  加班下班
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace("出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace("出差", "");
                            }
                            else if ((SourceTable.Rows[i]["StartWorkOvertime"].ToString() != "" && SourceTable.Rows[i]["EndWorkOvertime"].ToString() != ""))
                            {
                                //加班上班  加班下班
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescM"] = SourceTable.Rows[i]["AttendanceDescM"].ToString().Replace("出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace(",出差", "");
                                SourceTable.Rows[i]["AttendanceDescN"] = SourceTable.Rows[i]["AttendanceDescN"].ToString().Replace("出差", "");
                            }
                        }
                        */
                        //員工
                        XSSFCell Cell = (XSSFCell)dataRow.CreateCell(j);
                        Cell.SetCellValue(SourceTable.Rows[i][DisplayList[j].ColumnName].ToString());

                        //遲到分鐘數 顏色判定
                        if (DisplayList[j].ColumnName == "LateMinFormat")
                        {
                            switch (SourceTable.Rows[i]["LateMin"].ToString())
                            {
                                case "": ; break;
                                case "0": ; break;
                                case "1": Cell.CellStyle = LateStyle5Min; ; break;
                                case "2": Cell.CellStyle = LateStyle5Min; ; break;
                                case "3": Cell.CellStyle = LateStyle5Min; ; break;
                                case "4": Cell.CellStyle = LateStyle5Min; ; break;
                                default: Cell.CellStyle = LateStyle; break;
                            }

                        }

                        //曠職 顏色判定
                        if (DisplayList[j].ColumnName == "AttendanceDescM")
                        {
                            string Tmp = SourceTable.Rows[i]["AttendanceDescM"].ToString();

                            if (Tmp.IndexOf("曠職") != -1)
                            {
                                Cell.CellStyle = AbsStyle;
                            }
                        }
                        if (DisplayList[j].ColumnName == "AttendanceDescN")
                        {
                            string Tmp = SourceTable.Rows[i]["AttendanceDescN"].ToString();

                            if (Tmp.IndexOf("曠職") != -1)
                            {
                                Cell.CellStyle = AbsStyle;
                            }
                        }
                    }
                    IntRow++;
                }

                /*
                    foreach (DataColumn column in SourceTable.Columns)
                    {
                        XSSFCell cell = (XSSFCell)dataRow.CreateCell(column.Ordinal);
                        cell.SetCellValue(p.ToString() +"-"+row[column].ToString());


                        switch(column.ColumnName.ToString())
                        {
                            case "LateMin":;break;
                        }

                */




                //加一筆空白行數



                //dataRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName.ToString()+"-"+);

            }

            /*
            XSSFRow row = (XSSFRow)sheet.CreateRow(1);


            XSSFCell cell;

            for (int i = 0; i < title.Length; i++)
            {
                
                cell = (XSSFCell)row.CreateCell(i + 2);
                cell.CellStyle = LateStyle5Min;
                cell.SetCellValue(title[i]);
            }
             */


            workbook.Write(ms);
            ms.Flush();
            //ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return ms;
        }


        /// <summary>
        /// 輸出員工出勤補登記錄表Excel
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <returns></returns>
        public Stream RA02ToExcel(ReportFormModel ReportFrom)
        {
            List<RA02Model> RG02List = new List<RA02Model>();

            string SQL = " SELECT * FROM vAttendanceCheckIn WHERE 1=1";
            if (ReportFrom.Company != "")
            {
                SQL += " AND Company = @Company ";
            }


            if (ReportFrom.EmpNo != "")
            {
                SQL += " AND EmployeeNo = @EmployeeNo ";
            }


            if (ReportFrom.DepartMentNo != "")
            {
                SQL += " AND DepartMentNo = @DepartMentNo ";
            }

            SQL += " AND CardDate BETWEEN @StartDate AND @EndDate AND Status = @Status";
            SQL += " Order by EmployeeNo,CardDate,CardTime ";

            DataTable dt = DbAccess.ExecuteDataTable(SQL,
                new DbParameter[] {
                    DataAccess.CreateParameter("Company", DbType.String, ReportFrom.Company.ToString()),
                    DataAccess.CreateParameter("EmployeeNo", DbType.String, ReportFrom.EmpNo.ToString()),
                    DataAccess.CreateParameter("DepartMentNo", DbType.String, ReportFrom.DepartMentNo.ToString()),
                    DataAccess.CreateParameter("StartDate", DbType.String, ReportFrom.StartDate.ToString()),
                    DataAccess.CreateParameter("EndDate", DbType.String, ReportFrom.EndDate.ToString()),
                    DataAccess.CreateParameter("Status", DbType.String, ReportFrom.EmpStatus.ToString())
                }
            );

            IWorkbook workbook = new XSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = (XSSFSheet)workbook.CreateSheet();
            IRow headerRow = (XSSFRow)sheet.CreateRow(0);


            //定義輸出欄位
            List<DataTableDisplay> DisplayList = new List<DataTableDisplay>();
            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeNo",
                DisplayName = "員工編號"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "DepartMentName",
                DisplayName = "部門"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeName",
                DisplayName = "姓名"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "EmployeeEName",
                DisplayName = "英文名"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CompanyName",
                DisplayName = "公司別"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CardDate",
                DisplayName = "打卡日期"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CardTime",
                DisplayName = "打卡時間"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CardType",
                DisplayName = "打卡類別"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CheckInDescription",
                DisplayName = "補登原因"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CheckInEmployeeName",
                DisplayName = "補登人"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CheckInDate",
                DisplayName = "補登日期"
            });

            DisplayList.Add(new DataTableDisplay()
            {
                ColumnName = "CheckInTime",
                DisplayName = "補登時間"
            });

            for (int i = 0; i < DisplayList.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(DisplayList[i].DisplayName);
            }

            int IntRow = 1;
            if (dt.Rows.Count > 0)
            {
                string BrEmpNo = dt.Rows[0]["EmployeeNo"].ToString();
                string Test = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (BrEmpNo != dt.Rows[i]["EmployeeNo"].ToString())
                    {
                        //換行!!
                        IRow tmpDataRow = (XSSFRow)sheet.CreateRow(IntRow);
                        XSSFCell tmpCell = (XSSFCell)tmpDataRow.CreateCell(0);
                        IntRow++;
                        BrEmpNo = dt.Rows[i]["EmployeeNo"].ToString();
                    }

                    IRow dataRow = (XSSFRow)sheet.CreateRow(IntRow);

                    for (int j = 0; j < DisplayList.Count; j++)
                    {
                        try
                        {
                            Test = dt.Rows[i][DisplayList[j].ColumnName].ToString();
                        }
                        catch
                        {
                            //查無此欄位 則空著!!
                            XSSFCell CellR = (XSSFCell)dataRow.CreateCell(j);
                            CellR.SetCellValue("");
                            continue;
                        }
                        XSSFCell Cell = (XSSFCell)dataRow.CreateCell(j);
                        Cell.SetCellValue(dt.Rows[i][DisplayList[j].ColumnName].ToString());
                    }
                    IntRow++;
                }
            }

            workbook.Write(ms);
            ms.Flush();

            sheet = null;
            headerRow = null;
            workbook = null;

            return ms;
        }
    }
}