using System.Text.RegularExpressions;
using Rynzo.Client.Features.Auth.Models;
namespace Rynzo.Client.Features.Auth.Validators;

public static partial class AuthFormValidator
{
    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();
    public static IReadOnlyDictionary<string, string> Validate(string flow, AuthFormState state)
    {
        var errors = new Dictionary<string, string>();
        if (flow is "sign-in" or "sign-up" or "forgot-password" && !EmailPattern().IsMatch(state.Email)) errors["email"] = "Enter a valid email address.";
        if (flow is "sign-in" or "sign-up" or "reset-password" or "lock-screen" && state.Password.Length == 0) errors["password"] = "Password is required.";
        if (flow is "sign-up" or "reset-password")
        {
            if (state.Password.Length < 8) errors["password"] = "Use at least 8 characters.";
            if (state.Password != state.ConfirmPassword) errors["confirm"] = "Passwords do not match.";
        }
        return errors;
    }
    public static int Strength(string value) => value.Length > 11 ? 4 : value.Length > 8 ? 3 : value.Length > 5 ? 2 : value.Length > 0 ? 1 : 0;
}
