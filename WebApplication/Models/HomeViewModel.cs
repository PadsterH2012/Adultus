using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class HomeViewModel
    {
        public List<userlist> userList { get; set; }

        public aspnetuserclaim aspUserClaim { get; set; }
    }
}