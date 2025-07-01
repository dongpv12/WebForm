using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
    public class IndexPortal
    {
        public ListNews LstNews { get; set; }
        public ListProject LstProject { get; set; }

        public IndexPortal()
        {
            LstNews = new ListNews();
            LstProject = new ListProject();
        }
    }
}