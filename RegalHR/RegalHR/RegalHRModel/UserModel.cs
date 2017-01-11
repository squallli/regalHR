using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegalHRModel
{
    public class UserModel : BaseModel
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
        /// 使用者編號
        /// </summary>
        public string UserId { get; set; }



        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }




        /// <summary>
        /// 使用者英文名稱
        /// </summary>
        public string UserEName { get; set; }





        /// <summary>
        /// 部門編號
        /// </summary>
        public string DepNo { get; set; }



        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DepName { get; set; }



        /// <summary>
        /// 權限群組ID
        /// </summary>
        public string GroupID { get; set; }



        /// <summary>
        /// 權限群組POWER值
        /// </summary>
        public string Power { get; set; }





        /// <summary>
        /// ProgramList
        /// </summary>
        public List<ProgramModel> ProgramList = new List<ProgramModel>();


        


        /// <summary>
        /// LDAP
        /// </summary>
        public LDAPUsersModel LDAP { get; set; }
        
    }
}
