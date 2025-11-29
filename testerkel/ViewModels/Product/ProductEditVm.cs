using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using testerkel.Models;

namespace testerkel.ViewModels.Product
{
    public class ProductEditVm
    {
        public required int Id { get; set; }

        [MaxLength(50)]
        public required string Code { get; set; }
        public string? Name { get; set; }

        [Range(0, 999999999999)]
        public decimal Price { get; set; }
        public required UnitType Unit { get; set; }

        public IEnumerable<SelectListItem>? UnitTypes { get; set; }
    }
}
