using System.ComponentModel.DataAnnotations;

namespace testerkel.Utility
{
    public class AllowedDenominatorAttribute : ValidationAttribute
    {
        private readonly string[] _denominators;
        public AllowedDenominatorAttribute(string[] denominators)
        {
            _denominators = denominators;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var price = value as decimal?;

            if (price.HasValue)
            {
                var priceString = price.Value.ToString();
                var hasValidDenominator = false;
                foreach (var denominator in _denominators)
                {
                    if (priceString.Contains(denominator))
                    {
                        hasValidDenominator = true;
                        break;
                    }
                }
                if (!hasValidDenominator && _denominators.Length > 0)
                {
                    var errorMessage = $"The price must contain one of the following denominators: {string.Join(", ", _denominators)}.";
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
