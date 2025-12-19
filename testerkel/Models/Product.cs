using testerkel.Utility;

namespace testerkel.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string? Name { get; set; }
        public UnitType Unit { get; set; }
    }
}
