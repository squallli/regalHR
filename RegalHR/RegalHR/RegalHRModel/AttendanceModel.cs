using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class AttendanceCheckInDtlModel
    {

        public EmpModel Emp { get; set; }

        public string CardType { get; set; }

        public string CardDate { get; set; }

        public string CardTime { get; set; }

        public string CheckInFlag { get; set; }

        public string CheckInEmployeeNo { get; set; }

        public string CheckInDate { get; set; }

        public string CheckInTime { get; set; }

        public string CheckInEmployeeName { get; set; }

        public string CheckInDescription { get; set; }
        
    }

    /// <summary>
    /// 顯示詳細資料用
    /// </summary>
    public class AttendanceShowDtlModel
    {
        public EmpModel Emp { get; set; }

        public HolidayDtlModel ThisDate { get; set; }

        //外出記錄
        public List<OutgoingEventModel> OutgoingList = new List<OutgoingEventModel>();

        //打卡記錄
        public List<AttendanceModel> AttendanceList = new List<AttendanceModel>();
    }

    //補登專用
    public class AttendanceCheckInModel : BaseModel
    {

    }

    public class AttendanceModel : BaseModel
    {

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 員工代碼
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 員工名稱
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 員工英文姓名
        /// </summary>
        public string EmployeeEName { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DepartMentName { get; set; }

        /// <summary>
        /// 上班日期
        /// </summary>
        public string WorkDay { get; set; }

        /// <summary>
        /// 上班時間
        /// </summary>
        public string StartWorkTime { get; set; }

        /// <summary>
        /// 下班時間
        /// </summary>
        public string EndWorkTime { get; set; }

        /// <summary>
        /// 加班上班時間
        /// </summary>
        public string StartWorkOvertime { get; set; }

        /// <summary>
        /// 加班下班時間
        /// </summary>
        public string EndWorkOvertime { get; set; }

        /// <summary>
        /// 遲到分鐘
        /// </summary>
        public string LateMin { get; set; }

        /// <summary>
        /// 遲到分鐘格式
        /// </summary>
        public string LateMinFormat { get; set; }

        /// <summary>
        /// 早退分鐘
        /// </summary>
        public string LeaveMin { get; set; }

        /// <summary>
        /// 出勤描述
        /// </summary>
        public string AttendanceDesc { get; set; }

        //1051129 Scott新增
        /// <summary>
        /// 出勤描述(早)
        /// </summary>
        public string AttendanceDescM { get; set; }

        //1051129 Scott新增
        /// <summary>
        /// 出勤描述(晚)
        /// </summary>
        public string AttendanceDescN { get; set; }

        //1051129 Scott新增
        /// <summary>
        /// 出勤描述(加班)
        /// </summary>
        public string AttendanceDescOT { get; set; }

        /// <summary>
        /// 出勤描述 (顯示給工程師看的)
        /// </summary>
        public string AttendanceDesc2 { get; set; }

        /// <summary>
        /// 卡鐘模式
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 卡鐘模式
        /// </summary>
        public string CardTypeName { get; set; }

        /// <summary>
        /// RecordLimit
        /// </summary>
        public string RecordLimit { get; set; }

        /// <summary>
        /// RowNumID
        /// </summary>
        public string RowNumID { get; set; }

        /// <summary>
        /// RowNumTotal
        /// </summary>
        public string RowNumTotal { get; set; }

        //查詢個人詳細資料用  補登用
        public string CheckInFlag { get; set; }

        public string CheckInEmployeeNo { get; set; }

        public string CheckInDate { get; set; }

        public string CheckInTime { get; set; }

        public string CheckInEmployeeName { get; set; }

        public string CheckInDescription { get; set; }

    }

}
