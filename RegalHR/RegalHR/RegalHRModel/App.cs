using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{




    /// <summary>
    /// 登入
    /// </summary>
    public class AppLoginModel
    {
        public string Result { get; set; }
        public string UserId { get; set; }//員工編號
        public string UserName { get; set; }//姓名
        public string UserEName { get; set; }//英文姓名
        public string Company { get; set; }//公司id
        public string CompanyName { get; set; }//公司名稱
    }











    /// <summary>
    /// 登入
    /// </summary>
    public class AppOutgoingModel
    {
        public string OutId { get; set; } //外出單據Id(編輯外出單據使用)


        public string OutMan { get; set; } //外出人
        public string OutManCompany { get; set; } //外出人 公司 (當需要串到企業臨時員工時 用到的)
        public string SDate { get; set; } //外出起始日期
        public string STime { get; set; } //外出起始時間
        public string Location { get; set; } //外出地點
        public string CustomerName { get; set; } //客戶名稱
        public string OutDescription { get; set; } //出勤描述
        public string GoOutTime { get; set; } //預計外出時間
    }







    /// <summary>
    /// 出勤以日為單位模組
    /// </summary>
    public class AppAttendanceModel
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
        /// 英文名稱
        /// </summary>
        public string EmployeeEName { get; set; }



        public string CardDate  { get; set; }


        public List<AppAttendanceCardTime> CardTimeList = new List<AppAttendanceCardTime>();

    }










    /// <summary>
    /// 出勤卡片
    /// </summary>
    public class AppAttendanceCardTime
    {


        public string CardTime { get; set; }

        /// <summary>
        /// 卡鐘模式  1. 上班  2.下班  3.加班上班  4.加班下班
        /// </summary>
        public string CardType { get; set; }

    }







    /// <summary>
    /// App版本下載
    /// </summary>
    public class AppVersionModel
    {
        public string AppOS { get; set; }
        public string Version { get; set; }
        public string Download { get; set; }
    }

}
