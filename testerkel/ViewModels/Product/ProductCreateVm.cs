using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using testerkel.Models;
using testerkel.Utility;

namespace testerkel.ViewModels.Product
{
    public class ProductCreateVm
    {
        public string? Name { get; set; }

        [MaxLength(50)]
        public required string Code { get; set; }
        public required UnitType Unit { get; set; }
        public IEnumerable<SelectListItem>? UnitTypes { get; set; }
    }
}
