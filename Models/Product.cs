using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ap103PartialView.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int? TagId { get; set; }
        public Tag Tag { get; set; }
        public List<ProductCategory> ProductCategories{ get; set; }
        public List<ProductColor> ProductColors { get; set; }
        [NotMapped]
        public IFormFile PhotoFile { get; set; }
        [NotMapped]
        public List<int> CategoryIds { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
    }
}
