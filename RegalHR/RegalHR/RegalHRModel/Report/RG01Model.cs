using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class RG01Model
    {

        /// <summary>
        /// 外出單號
        /// </summary>
        public string OutId { get; set; }

        /// <summary>
        /// 單據狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 公司別
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 部門別
        /// </summary>
        public string DepartMentName { get; set; }

        /// <summary>
        /// 外出員工編號
        /// </summary>
        public string OutMan { get; set; }

        /// <summary>
        /// 外出者姓名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 外出日期
        /// </summary>
        public string OutDate { get; set; }

        /// <summary>
        /// 外出時間
        /// </summary>
        public string OutTime { get; set; }

        /// <summary>
        /// 外出地點
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 異常次數
        /// </summary>
        public string AlermTotal { get; set; }

        /// <summary>
        /// 預計出門
        /// </summary>
        public string GoOutTime { get; set; }

        public List<RG01DtlModel> RG01Dtl = new List<RG01DtlModel>();
    }

    public class RG01DtlModel
    {
        /// <summary>
        /// 該筆狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 外出日期
        /// </summary>
        public string OutDate { get; set; }

        /// <summary>
        /// 外出時間
        /// </summary>
        public string OutTime { get; set; }

        /// <summary>
        /// 外出地點
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 預計出門
        /// </summary>
        public string GoOutTime { get; set; }

        /// <summary>
        /// 最後編輯日期
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 最後編輯時間
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 最後編輯者
        /// </summary>
        public string RecordManName { get; set; }

        /// <summary>
        /// 異常狀態
        /// </summary>
        public string Alerm { get; set; }
    }
}
