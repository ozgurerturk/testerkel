using Microsoft.AspNetCore.Mvc.Rendering;

namespace testerkel.ViewModels.SalesOrder
{
    public class SalesOrderCreateVm
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public required decimal Qty { get; set; }
        public required decimal UnitPrice { get; set; }

        public IEnumerable<SelectListItem>? Customers { get; set; }
        public IEnumerable<SelectListItem>? Products { get; set; }
    }
}
