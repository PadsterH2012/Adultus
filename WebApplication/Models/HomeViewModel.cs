using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class HomeViewModel
    {
        public List<userlist> userList { get; set; }

        public List<aspnetuserclaim> aspUserClaim { get; set; }

        public List<aspnetrole> aspRole { get; set; }
    }
}