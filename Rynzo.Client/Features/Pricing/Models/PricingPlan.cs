namespace Rynzo.Client.Features.Pricing.Models;

public enum BillingCycle { Monthly, Yearly }
public sealed record PricingPlan(string Id, string Name, int? Monthly, int? Yearly, string Description, bool Featured, IReadOnlyList<string> Features)
{
    public int? Price(BillingCycle cycle) => cycle == BillingCycle.Monthly ? Monthly : Yearly;
}
public sealed record PricingComparison(string Group, string Feature, IReadOnlyList<string> Values);
public sealed record PricingFaq(string Question, string Answer);
public sealed class SalesDraft
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Company { get; set; } = "";
    public string TeamSize { get; set; } = "";
    public string Message { get; set; } = "";
}
