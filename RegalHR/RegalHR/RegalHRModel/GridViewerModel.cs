using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class GridViewerModel
    {

        /// <summary>
        /// 總筆數
        /// </summary>
        public int RecordTotal { get; set; }


        /// <summary>
        /// 總頁數
        /// </summary>
        public int PageTotal { get; set; }

        /// <summary>
        /// 目前是第幾頁
        /// </summary>
        public int Page { get; set; }


        /// <summary>
        /// 連結URL
        /// </summary>
        public string TargetURL { get; set; }



        /// <summary>
        /// 分頁顯示幾筆
        /// </summary>
        public int PageLimit { get; set; }

        

        
        /// <summary>
        /// GridViewer資料
        /// </summary>
        public List<BaseModel> DataList = new List<BaseModel>();




        /// <summary>
        /// 計算總頁數
        /// </summary>
        /// <param name="Total">總資料筆數</param>
        /// <param name="Limit">一頁顯示幾筆</param>
        /// <returns>總頁</returns>
        public int GetPageTotal(int Total, int Limit)
        {
            int temp = Total / Limit;
            if (Total != 0 && Total % Limit != 0)
            {
                temp++;
            }

            return temp;
        }
        
    }
}
