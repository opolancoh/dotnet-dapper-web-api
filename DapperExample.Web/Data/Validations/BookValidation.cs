using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DapperExample.Web.Data.Validations;

public static class BookValidation
{
    public static IEnumerable<ValidationResult> ValidateTitle(string value, string fieldName)
    {
        if (!Regex.IsMatch(value, "^[a-zA-Z0-9 ()']*$"))
        {
            yield return new ValidationResult(
                $"The {fieldName} field is invalid.", new[] {fieldName});
        }
    }
}