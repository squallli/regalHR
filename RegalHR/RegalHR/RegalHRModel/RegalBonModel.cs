using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class EmpOutingInfoModel : BaseModel
    {


        /// <summary>
        /// 員工代碼
        /// </summary>
        public string EmployeeNo { get; set; }



        /// <summary>
        /// 員工名稱
        /// </summary>
        public string EmployeeName { get; set; }


        /// <summary>
        /// 員工英文名稱
        /// </summary>
        public string EmployeeEName { get; set; }




        /// <summary>
        /// 外出日期
        /// </summary>
        public string OutgoingDate { get; set; }



        public List<OutgoingListModel> OutgoingList = new List<OutgoingListModel>();

    }



    public class OutgoingListModel : BaseModel
    {
        public string OutId { get; set; } //外出單id
        public string OutDate { get; set; } //外出日期
        public string OutTime { get; set; } //外出時間
        public string OutMan { get; set; } //外出人
        public string Location { get; set; } //外出地點
        public string CustomerName { get; set; } //客戶名稱
        public string GoOutTime { get; set; } //出門時間


        public string UpdateDate { get; set; } //更新日期
        public string UpdateTime { get; set; } //更新時間     

    }




    public class TempLoginModel 
    {
        public string Rid { get; set; } //臨時ID
        public string StartTime { get; set; } //起始時間
        public string EndTime { get; set; } //時效性
        public string EmpNo { get; set; } //員工編號
        public int CountDown { get; set; } //倒數
    }
}
