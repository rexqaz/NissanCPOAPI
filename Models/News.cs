//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class News
    {
        public long seq { get; set; }
        public string title { get; set; }
        public string bodyContent { get; set; }
        public string picturePath { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public string status { get; set; }
        public string publishRange { get; set; }
        public string brand { get; set; }
        public string showBody { get; set; }
    }
}