namespace testerkel.Models
{
    public class Consumption
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public DateTime ConsumptionDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        public IList<ConsumptionLine> Lines { get; set; } = new List<ConsumptionLine>();
    }

    public class ConsumptionLine
    {
        public int Id { get; set; }

        public int ConsumptionId { get; set; }
        public Consumption? Consumption { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal Qty { get; set; }
    }
}
