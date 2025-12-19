namespace testerkel.Models
{
    public class StockTxn
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public decimal Qty { get; set; }                 // Her zaman pozitif

        public decimal? UnitPrice { get; set; }   // özellikle PurchaseIn için
        public decimal? LineTotal { get; set; }   // istersen UnitPrice * Qty (opsiyonel)

        public StockDirection Direction { get; set; }    // tinyint
        public StockMovementType MovementType { get; set; }

        public DateTime TxnDate { get; set; } = DateTime.UtcNow;

        public string? RefModule { get; set; }           // "PO", "SO", "SF", "TRF"
        public int? RefId { get; set; }
    }
}
