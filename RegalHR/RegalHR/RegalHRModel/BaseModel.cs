using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{

    public class BaseModel
    {


        /// <summary>
        /// 修改專用時 PK值
        /// </summary>
        public string EditPK { get; set; }

        /// <summary>
        /// 模式:  ADD新增  EDIT:修改
        /// </summary>
        public string Mode { get; set; }

    }
}
