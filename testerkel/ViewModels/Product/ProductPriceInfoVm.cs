using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using testerkel.Models;
using testerkel.ViewModels.Stock;

namespace testerkel.ViewModels.Product
{
    public class ProductPriceInfoVm
    {
        public required int Id { get; set; }

        [MaxLength(50)]
        public required string Code { get; set; }
        public string? Name { get; set; }
        public required UnitType Unit { get; set; }

        public decimal? PriceAverage { get; set; }

        public decimal? LastPrice { get; set; }

        public List<StockTxnRowVm> MovementHistory { get; set; } = [];
    }
}
