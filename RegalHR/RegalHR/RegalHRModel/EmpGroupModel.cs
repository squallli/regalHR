using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class EmpGroupModel : BaseModel
    {

        public EmpModel Emp = new EmpModel();

        public List<GroupModel> GroupList = new List<GroupModel>();

        public List<GroupModel> NoGroupList = new List<GroupModel>();

    }

    
}
