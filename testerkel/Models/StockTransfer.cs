namespace testerkel.Models
{
    public class StockTransfer
    {
        public int Id { get; set; }

        public int FromWarehouseId { get; set; }
        public Warehouse? FromWarehouse { get; set; }

        public int ToWarehouseId { get; set; }
        public Warehouse? ToWarehouse { get; set; }

        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        public IList<StockTransferLine> Lines { get; set; } = new List<StockTransferLine>();
    }

    public class StockTransferLine
    {
        public int Id { get; set; }

        public int TransferId { get; set; }
        public StockTransfer? Transfer { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal Qty { get; set; }
    }
}
