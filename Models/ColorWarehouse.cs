using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
    public class ColorWarehouseRequest
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public int Lstord { get; set; }
    }

    public class ColorWarehouse : ColorWarehouseRequest
    {
        public int Id { get; set; }
        public string CreateDate { get; set; }
    }

    public class SearchColorWarehouseRequest
    {
        public int CurrentPage { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class ListColorWarehouse
    {
        public List<ColorWarehouse> Collection { get; set; }
        public int Start { get; set; }
        public string Paging { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
}