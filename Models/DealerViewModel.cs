using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class DealerViewModel
    {
        public List<Dealers> dealers { get; set; }
        public string type { get; set; }
        public string seq { get; set; }
        public string role { get; set; }
    }
}