using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class JsonresultModel
    {
        public string Result { get; set; }
        public string ErrorMsg { get; set; }
        public object Query { get; set; }
    }
}
