using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class CalendarEventModel : BaseModel
    {


        public string id { get; set; }
        public bool self { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        
        public string end { get; set; }
        public bool allDay { get; set; }
        public string backgroundColor { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
        public string className { get; set; }
        
        public bool editable { get; set; }
        
        
        public string UpdateDate { get; set; } //更新日期
        public string UpdateTime { get; set; } //更新時間
        public string RecordMan { get; set; } //登記人
        public string EmployeeNo { get; set; } //首頁專用
        public string EmployeeName { get; set; } //首頁專用
        public string EmployeeEName { get; set; } //首頁專用
        public string OwnFlag { get; set; } //首頁專用
        public string Status { get; set; } //狀態
        public string GoOutTime { get; set; } //預計外出時間



        
    }

    

    public class CalendarAllEventModel : CalendarEventModel
    {
        public List<CalendarEventModel> EventList { get; set; } //清單
    }





    public class OutgoingEventModel : CalendarEventModel
    {
        public string OutId { get; set; } //外出單id
        public string OutDate { get; set; } //外出日期
        public string OutTime { get; set; } //外出時間
        public string GoOutTime { get; set; } //預計出門時間

        public string OutMan { get; set; } //外出人
        public string Location { get; set; } //外出地點
        public string CustomerName { get; set; } //客戶名稱
        public string Status { get; set; } //狀態 A:新增 , D:刪除 , U:修改
        public string IsEdit { get; set; } //可否編輯
        public string Company { get; set; } //外出人 公司 (當需要串到企業臨時員工時 用到的)
        public string OutDescription { get; set; } //出勤描述
        public string Equipment { get; set; } //是否為設備


        
    }

    




    //表單條件使用
    public class OutgoingFormModel
    {
        public string RecordMan { get; set; } //登記人
        public string OutMan { get; set; } //外出人
        public string OutManCompany { get; set; } //外出人 公司 (當需要串到企業臨時員工時 用到的)
        public string SDate { get; set; } //外出起始日期
        public string STime { get; set; } //外出起始時間
        public string EDate { get; set; } //外出結束時間
        public string OutDescription { get; set; } //出勤描述
    }
}
