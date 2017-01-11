using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class ReportModel
    {
        public string ReportId { get; set; }
        public string URL { get; set; }
        public string ViewLevel { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// 回傳給網頁 給予最終excel 下載位址
    /// </summary>
    public class ExcelModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class ReportFormModel
    {

        /// <summary>
        /// 公司代碼
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 部門代碼
        /// </summary>
        public string DepartMentNo { get; set; }

        /// <summary>
        /// 員工編號
        /// </summary>
        public string EmpNo { get; set; }

        /// <summary>
        /// 日期區間 起始
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 日期區間 結束
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 是否離職
        /// </summary>
        public string EmpStatus { get; set; }

        public ReportFormModel()
        {
            Condition = new Dictionary<string, object>();
        }

        public Dictionary<string, Object> Condition { get; set; }

    }

    /// <summary>
    /// 輸出Excel title用
    /// </summary>
    public class DataTableDisplay
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
    }

}
