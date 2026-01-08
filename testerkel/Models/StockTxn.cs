namespace testerkel.Models
{
    public class StockTxn
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal Qty { get; set; }

        public decimal? UnitPrice { get; set; }
        public decimal? LineTotal { get; set; }

        public StockDirection Direction { get; set; }
        public StockMovementType MovementType { get; set; }

        public DateTime TxnDate { get; set; } = DateTime.UtcNow;

        public string? RefModule { get; set; }           // "PO", "SO", "SF", "TRF"
        public int? RefId { get; set; }
    }
}
