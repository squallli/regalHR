using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class EmpModel : BaseModel
    {



        /// <summary>
        /// 所屬的公司ID
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// 所屬的公司名稱
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
        /// 員工英文名稱
        /// </summary>
        public string EmployeeEName { get; set; }



        

        /// <summary>
        /// 到職日期
        /// </summary>
        public string dayofduty { get; set; }



        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 部門代碼
        /// </summary>
        public string DepartMentNo { get; set; }



        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DepartMentName { get; set; }




        /// <summary>
        /// 卡是否註銷  1=啟用  0=註銷  (此為UI使用)
        /// </summary>
        public string CardStatus { get; set; }



        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 卡號生效日
        /// </summary>
        public string CardEffectiveDate { get; set; }



        /// <summary>
        /// 卡號失效日
        /// </summary>
        public string CardExpiryDate { get; set; }




        /// <summary>
        /// 臨時員工編號
        /// </summary>
        public string TempEmployeeNo { get; set; }


        /// <summary>
        /// 修改旗標
        /// </summary>
        public string ModifyFlag { get; set; }





        /// <summary>
        /// 工作天數 依照天數為單位
        /// </summary>
        public string WorkDuration { get; set; }


        /// <summary>
        /// 離職日
        /// </summary>
        public string offDutyDate { get; set; }




        /// <summary>
        /// 男女旗標
        /// </summary>
        public string Sex { get; set; }




        /// <summary>
        /// 權限群組
        /// </summary>
        public List<GroupModel> Group = new List<GroupModel>();
        

        
    }
}
