public class StockTransferLineVm
{
    public int ProductId { get; set; }
    public decimal Qty { get; set; }
}

public class StockTransferEditVm
{
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime TransferDate { get; set; } = DateTime.Today;
    public string? Notes { get; set; }

    public List<StockTransferLineVm> Lines { get; set; } = new List<StockTransferLineVm>();
}
