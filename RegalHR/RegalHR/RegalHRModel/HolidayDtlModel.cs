using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class HolidayDtlModel : CalendarEventModel
    {



        /// <summary>
        /// 年
        /// </summary>
        public string YearId { get; set; }


        /// <summary>
        /// 所屬的公司
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// Ctype
        /// </summary>
        public string Ctype { get; set; }



        /// <summary>
        /// Ctype (明文)
        /// </summary>
        public string CtypeName { get; set; }



        /// <summary>
        /// 假期
        /// </summary>
        public string Holiday { get; set; }


        /// <summary>
        /// 是否顯示在日曆上
        /// </summary>
        public string Display { get; set; }



        /// <summary>
        /// 是否顯示在日曆上 (明文)
        /// </summary>
        public string DisplayName { get; set; }



        /// <summary>
        /// 標題
        /// </summary>
        public string Memo { get; set; }




    }
}
