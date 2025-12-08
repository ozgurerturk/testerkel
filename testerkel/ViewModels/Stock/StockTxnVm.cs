namespace testerkel.ViewModels.Stock
{
    public class StockTxnVm
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public decimal Qty { get; set; }
        public DateTime TxnDate { get; set; }
        public string? Note { get; set; }

        // For UI purposes
        public string PageTitle { get; set; } = "";
        public string SubmitText { get; set; } = "";
    }
}
