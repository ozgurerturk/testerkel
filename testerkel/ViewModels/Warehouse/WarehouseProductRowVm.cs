namespace testerkel.ViewModels.Warehouse
{
    public class WarehouseProductRowVm
    {
        public int ProductId { get; set; }
        public string Code { get; set; } = "";
        public string? Name { get; set; }
        public string Unit { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
