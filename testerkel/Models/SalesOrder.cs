namespace testerkel.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Draft";   // Draft, Confirmed

        public IList<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();
    }

    public class SalesOrderLine
    {
        public int Id { get; set; }

        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
