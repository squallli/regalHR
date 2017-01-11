using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class ProgramModel : BaseModel
    {

        public string ProgID { get; set; }
        public string ProgName { get; set; }
        public int Power { get; set; }
        public string Url { get; set; }



        public string FlagType { get; set; }
        public string ViewLevel { get; set; }
        public string ChkFlag { get; set; }
    }


}
