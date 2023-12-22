using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class MemberViewModel
    {
        public List<Members> model { get; set; }
        public string role { get; set; }
    }

    public class MemberUpdateViewModel
    {
        public Members member { get; set; }
        public string role { get; set; }
        public bool updatePermission { get; set; }
    }
}