using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string  Password { get; set; }
    }

 


    public class Allcode_Info
    {
        public string CdName { get; set; }
        public string CdType { get; set; }
        public string CdValue { get; set; }
        public string CdContent { get; set; }
        public decimal Lstodr { get; set; }

    }
}