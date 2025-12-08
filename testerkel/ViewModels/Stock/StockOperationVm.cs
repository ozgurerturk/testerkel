namespace testerkel.ViewModels.Stock
{
    public enum StockOperationType
    {
        PurchaseIn = 1,
        SalesOut = 2,
        ConsumptionOut = 3,
        Transfer = 4
    }

    public class StockOperationVm
    {
        public int WarehouseId { get; set; }
        public StockOperationType Operation { get; set; }
    }
}
