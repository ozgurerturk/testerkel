using testerkel.Models;

namespace testerkel.ViewModels.Stock
{
    public class StockTxnRowVm
    {
        public DateTime TxnDate { get; set; }
        public StockDirection Direction { get; set; }
        public StockMovementType MovementType { get; set; }

        public decimal Qty { get; set; }
        public decimal? UnitPrice { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = "";

        public string? RefModule { get; set; }
        public int? RefId { get; set; }
        public string? Note { get; set; }
        public int ProductId { get; internal set; }
    }
}
