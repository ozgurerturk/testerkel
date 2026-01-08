using System.ComponentModel.DataAnnotations;

namespace testerkel.ViewModels.Stock
{
    public class StockTxnVm
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Miktar negatif olamaz")]
        public int Qty { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Birim fiyat negatif olamaz")]
        public decimal UnitPrice { get; set; }
        public DateTime TxnDate { get; set; }
        public string? Note { get; set; }

        // For UI purposes
        public string PageTitle { get; set; } = "";
        public string SubmitText { get; set; } = "";
    }
}
