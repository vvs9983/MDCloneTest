using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Test.Validators
{
    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((value ?? "").ToString()))
            {
                return new ValidationResult(false, "Field is required.");
            }

            return !Regex.IsMatch((string)value, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$")
                ? new ValidationResult(false, "Invalid email address entered.")
                : ValidationResult.ValidResult;
        }
    }
}
