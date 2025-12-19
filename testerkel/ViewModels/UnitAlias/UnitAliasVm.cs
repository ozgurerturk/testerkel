using System.ComponentModel.DataAnnotations;
using testerkel.Models;

namespace testerkel.ViewModels.UnitAlias
{
    public class UnitAliasVm
    {
        public int Id { get; set; }
        public UnitType UnitType { get; set; }

        [Required, MaxLength(50)]
        public string Alias { get; set; } = "";
    }
}
