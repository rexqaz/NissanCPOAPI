using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class GroupViewModel
    {
        public List<Group> groups { get; set; }
        public string roleId { get; set; }
        public bool status { get; set; }
    }

    public class Group
    { 
        public string id { get; set; }
        public string menuName { get; set; }
        public bool isChecked {get;set;}
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        public string roleName { get; set; }
        public bool status { get; set; }

        public List<Group> subMenus { get; set; }
    }
}