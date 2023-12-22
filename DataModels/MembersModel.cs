using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.DataModels
{
    [Table("Members")]
    public partial class MembersModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long seq { get; set; }
        [MaxLength(50)]
        public string name { get; set; }
        [MaxLength(10)]
        public string id { get; set; }
        [MaxLength(10)]
        public string mobile { get; set; }
        [MaxLength(50)]
        public string email { get; set; }
        [MaxLength(50)]
        public string title { get; set; }
        [MaxLength(20)]
        public string birthday { get; set; }
        [MaxLength(100)]
        public string address { get; set; }
        [MaxLength(50)]
        public string interestedCar { get; set; }
        public bool? isMailVerify { get; set; }
        public bool? isMobileVerify { get; set; }
        [MaxLength(50)]
        public string password { get; set; }
        public DateTime? createTime { get; set; }
        public DateTime? updateTime { get; set; }
        public bool? needToChangeFirst { get; set; }
        public bool? status { get; set; }
        [MaxLength(10)]
        public string brand { get; set; }
        [MaxLength(10)]
        public string area { get; set; }
        [MaxLength(10)]
        public string loginStatus { get; set; }
        [MaxLength]
        public string active_token { get; set; }
        [MaxLength]
        public string refresh_token { get; set; }
    }
}