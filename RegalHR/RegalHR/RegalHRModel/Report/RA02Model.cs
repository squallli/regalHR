using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class RA02Model
    {

        /// <summary>
        /// 打卡日期
        /// </summary>
        public string CardDate { get; set; }

        /// <summary>
        /// 打卡時間
        /// </summary>
        public string CardTime { get; set; }

        /// <summary>
        /// 打卡類別
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 員工編號
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 員工名稱
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 補登者名稱
        /// </summary>
        public string CheckInEmployeeName { get; set; }

        /// <summary>
        /// 補登原因
        /// </summary>
        public string CheckInDescription { get; set; }

        /// <summary>
        /// 最後編輯日
        /// </summary>
        public string CheckInDate { get; set; }

        /// <summary>
        /// 最後編輯時間
        /// </summary>
        public string CheckInTime { get; set; }

        /// <summary>
        /// 員工英文名
        /// </summary>
        public string EnEmployeeName { get; set; }
    }

}
