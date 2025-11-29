using Microsoft.AspNetCore.Mvc.Rendering;
using testerkel.Models;

namespace testerkel.ViewModels.SalesOrder
{
    public class SalesOrderDetailVm
    {
        public int Id { get; set; }
        public required string CustomerName { get; set; }
        public required DateTime OrderDate { get; set; }
        public required string Status { get; set; }

        public IList<SalesOrderLineVm> Lines { get; set; } = new List<SalesOrderLineVm>();

        public IEnumerable<SelectListItem>? Warehouses { get; set; }
    }

    public class SalesOrderLineVm
    {
        public required string ProductName { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
