using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
    public class PageRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public string CreateBy { get; set; }
    }

    public class Page : PageRequest
    {
        public int Id { get; set; }
        public string CreateDate { get; set; }
    }

    public class ListPage
    {
        public List<Page> Collection { get; set; }
        public int Start { get; set; }
        public string Paging { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }

    public class SearchPageRequest
    {
        public int CurrentPage { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string OrderBy { get; set; }
        public string OrderByType { get; set; }
        public string CategoryType { get; set; }
        public string CreateDate { get; set; }
    }
}