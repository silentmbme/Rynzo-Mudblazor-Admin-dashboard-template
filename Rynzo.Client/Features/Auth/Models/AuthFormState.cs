namespace Rynzo.Client.Features.Auth.Models;
// In-memory form state only; no API DTO, credential cache or authenticated identity.
public sealed class AuthFormState
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
    public bool RememberMe { get; set; }
    public bool AcceptTerms { get; set; }
    public void ClearCredentials() { Password = ""; ConfirmPassword = ""; }
}
