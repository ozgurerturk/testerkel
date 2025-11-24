namespace testerkel.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        public string SupplierName { get; set; } = "";

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public string? Notes { get; set; }

        public IList<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
    }

    public class PurchaseOrderLine
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
