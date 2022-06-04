using Ap103PartialView.Models;
using System.Collections.Generic;

namespace Ap103PartialView.ViewModels
{
    public class HomeVM
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }
}
