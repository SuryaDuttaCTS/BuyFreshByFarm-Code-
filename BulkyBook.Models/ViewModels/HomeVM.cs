using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
namespace BulkyBook.Models.ViewModels
{
   public class HomeVM
    {
        public IEnumerable<Product> productList;
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
