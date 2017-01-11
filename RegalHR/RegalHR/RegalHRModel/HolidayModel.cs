using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class HolidayModel : BaseModel
    {

        /// <summary>
        /// 年
        /// </summary>
        public string YearId { get; set; }

        /// <summary>
        /// 所屬的公司ID
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// 所屬的公司名稱
        /// </summary>
        public string CompanyName { get; set; }



        /// <summary>
        /// 假期明細檔案
        /// </summary>
        public List<HolidayDtlModel> HolidayDtlList = new List<HolidayDtlModel>();
        

        
    }
}
