using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForm.Models
{
 

    public class  ProductsRequest
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int GroupId { get; set; }
        public string Detail { get; set; }
        public string Avatar { get; set; }
        public int CoatingThickness { get; set; }
        public string Features1 { get; set; }
        public string Features2 { get; set; }
        public string Features3 { get; set; }
        public string Features4 { get; set; }
        public decimal Price { get; set; }
    }

    public class Products : ProductsRequest
    {
        public decimal Id { get; set; }
        public string GroupName { get; set; }
    }

    public class SearchProductRequest
    {
        public int CurrentPage { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string OrderBy { get; set; }
        public string OrderByType { get; set; }
    }

    public class SearchProductPortalRequest: SearchProductRequest
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
    public class ListProducts
    {
        public List<Products> Collection { get; set; }
        public int Start { get; set; }
        public string Paging { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }

    public class ListProductsPortal: ListProducts
    {
        public string PageName { get; set; }
        public string Router { get; set; }
    }



    public class GroupProductsRequest
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
    }


    public class GroupProducts 
    {
        public decimal GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
    }

    public class SearchGroupProductRequest
    {
        public int CurrentPage { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string OrderBy { get; set; }
        public string OrderByType { get; set; }
    }

    public class ListGroupProducts
    {
        public List<GroupProducts> Collection { get; set; }
        public int Start { get; set; }
        public string Paging { get; set; }
        public int TotalRecord { get; set; }
        public decimal TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
}