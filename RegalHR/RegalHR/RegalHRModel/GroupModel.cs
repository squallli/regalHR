using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class GroupModel : BaseModel
    {

        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public int Power { get; set; }
        public string ViewLevel { get; set; }
        public string ViewLevelName { get; set; }
        public List<ProgramModel> GroupProgramList = new List<ProgramModel>();


    }

    
}
