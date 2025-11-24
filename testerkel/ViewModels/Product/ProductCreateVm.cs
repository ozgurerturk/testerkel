using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using testerkel.Models;

namespace testerkel.ViewModels.Product
{
    public class ProductCreateVm
    {
        public string? Name { get; set; }
        public required string Code { get; set; }
        [Range(0, 999999999999)]
        public decimal Price { get; set; }
        public required UnitType Unit { get; set; }
        public IEnumerable<SelectListItem>? UnitTypes { get; set; }
    }
}
