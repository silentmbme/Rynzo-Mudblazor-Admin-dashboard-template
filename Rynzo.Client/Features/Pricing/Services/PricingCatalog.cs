using Rynzo.Client.Features.Pricing.Models;
namespace Rynzo.Client.Features.Pricing.Services;
// Exact original local display content; not an API response or production billing contract.
public static class PricingCatalog
{
    public static IReadOnlyList<PricingPlan> Plans { get; } = Array.AsReadOnly<PricingPlan>([
        new("starter", "Starter", 19, 15, "For individuals and small teams.", false, Array.AsReadOnly<string>(["Up to 5 team members", "10 projects", "Basic analytics", "5 GB storage", "Email support", "Standard integrations"])),
        new("growth", "Growth", 49, 39, "For growing product teams.", true, Array.AsReadOnly<string>(["Up to 25 team members", "Unlimited projects", "Advanced analytics", "50 GB storage", "Priority support", "Workflow automation", "Advanced integrations"])),
        new("business", "Business", 99, 79, "For larger organizations.", false, Array.AsReadOnly<string>(["Up to 100 team members", "Unlimited projects", "Advanced reporting", "250 GB storage", "Role-based permissions", "SSO & audit logs", "Priority support"])),
        new("enterprise", "Enterprise", null, null, "For organisations with advanced requirements.", false, Array.AsReadOnly<string>(["Unlimited team members", "Enterprise security", "Custom roles", "Dedicated account manager", "SLA", "Advanced compliance", "Custom integrations", "Migration assistance"]))
    ]);
    public static IReadOnlyList<PricingComparison> Comparison { get; } = Array.AsReadOnly<PricingComparison>([
        new("Core", "Team members", Array.AsReadOnly<string>(["5", "25", "100", "Unlimited"])),
        new("Core", "Projects", Array.AsReadOnly<string>(["10", "Unlimited", "Unlimited", "Unlimited"])),
        new("Core", "Storage", Array.AsReadOnly<string>(["5 GB", "50 GB", "250 GB", "Custom"])),
        new("Productivity", "Analytics", Array.AsReadOnly<string>(["Basic", "Advanced", "Advanced reporting", "Custom"])),
        new("Productivity", "Automation", Array.AsReadOnly<string>(["", "Included", "Included", "Included"])),
        new("Productivity", "Integrations", Array.AsReadOnly<string>(["Standard", "Advanced", "Advanced", "Custom"])),
        new("Security", "SSO", Array.AsReadOnly<string>(["", "", "Included", "Included"])),
        new("Security", "Audit logs", Array.AsReadOnly<string>(["", "", "Included", "Included"])),
        new("Support", "Priority support", Array.AsReadOnly<string>(["", "Included", "Included", "Included"])),
        new("Support", "Dedicated manager", Array.AsReadOnly<string>(["", "", "", "Included"]))
    ]);
    public static IReadOnlyList<PricingFaq> Faqs { get; } = Array.AsReadOnly<PricingFaq>([
        new("Can I change plans later?", "Yes. Upgrade, downgrade, or change billing frequency whenever your needs change."),
        new("Do you offer a free trial?", "Every Starter, Growth, and Business plan includes a 14-day trial with no credit card required."),
        new("Can I cancel anytime?", "Yes. Your plan remains active until the end of the current billing period."),
        new("What happens if I exceed plan limits?", "We will notify workspace admins and help you select the right capacity before access is affected."),
        new("Is SSO included?", "SSO and audit logs are included with Business and Enterprise."),
        new("Do you offer enterprise discounts?", "Annual Enterprise agreements are tailored by team size, security, and support requirements.")
    ]);
}
