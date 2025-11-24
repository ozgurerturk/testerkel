namespace testerkel.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? TaxNo { get; set; }

        public IList<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
