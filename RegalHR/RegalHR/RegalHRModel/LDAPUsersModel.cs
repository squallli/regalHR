using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class LDAPUsersModel
    {


        public string DomainAccount { get; set; }

        public string Domain { get; set; }

        public string sAMAccountName { get; set; }

        public string sAMAccountType { get; set; }

        public string objectGUID { get; set; }

        public string mail { get; set; }

        public string Path { get; set; }

        public string IsOff { get; set; }

        public string description { get; set; }

        

    }
}
