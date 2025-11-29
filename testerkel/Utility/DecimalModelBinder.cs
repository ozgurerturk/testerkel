using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

namespace testerkel.Utility
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            var value = context.ValueProvider.GetValue(context.ModelName).ToString();

            if (string.IsNullOrWhiteSpace(value))
            {
                context.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // Hepsini TR ondalık formatına çevir: "." -> ","
            value = value.Replace(".", ",");

            if (decimal.TryParse(value, NumberStyles.Number, new CultureInfo("tr-TR"), out var parsed))
            {
                context.Result = ModelBindingResult.Success(parsed);
            }
            else
            {
                context.ModelState.AddModelError(context.ModelName, "Geçerli bir fiyat girin.");
            }

            return Task.CompletedTask;
        }
    }
}
