using System.ComponentModel.DataAnnotations;

namespace testerkel.ViewModels.Warehouse
{
    public class WarehouseEditVm
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = "";

        [MaxLength(200)]
        public string? Name { get; set; }

        public bool IsActive { get; set; } = true;

        public IEnumerable<WarehouseProductRowVm> Products { get; set; }
            = [];
    }
}
