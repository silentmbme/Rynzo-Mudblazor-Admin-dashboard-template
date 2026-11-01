using Rynzo.Client.Features.Help.Models;
namespace Rynzo.Client.Features.Help.Services;

public static class FaqCatalog
{
    // Exact local content from React FaqApp; these are not API DTOs/responses.
    public static IReadOnlyList<FaqArticle> Articles { get; } = Array.AsReadOnly<FaqArticle>([
        new("first-project", "Getting Started", "How do I create my first project?", "Open Projects, select Create Project, add a name and owner, then invite the people who will work on it.", Array.AsReadOnly<string>(["new project", "create", "start"])),
        new("invite", "Getting Started", "How do I invite a team member?", "Open the Team page, choose Invite Member, enter their email address and assign a role. They will receive an invitation to join the workspace.", Array.AsReadOnly<string>(["team", "invite", "member"])),
        new("dashboards", "Getting Started", "How do I switch between dashboards?", "Use the Dashboards item in the sidebar and select the workspace that best matches the work you want to review.", Array.AsReadOnly<string>(["dashboard", "overview", "switch"])),
        new("profile", "Account & Profile", "How do I update my profile information?", "Visit Profile from the account menu, edit your details, and save. Changes are reflected across your workspace.", Array.AsReadOnly<string>(["profile", "name", "avatar"])),
        new("password", "Account & Profile", "How do I change my password?", "Open Settings, choose Security, then use Change password. For a secure account, choose a unique password you do not reuse elsewhere.", Array.AsReadOnly<string>(["password", "security", "login"])),
        new("project-members", "Projects & Tasks", "How do I add members to a project?", "Open the project details page, select the team area, then add members and choose their project role.", Array.AsReadOnly<string>(["project", "members", "team"])),
        new("kanban", "Projects & Tasks", "How do I move a task between Kanban stages?", "Drag the task card to its new column, or open it and update its status from the task details panel.", Array.AsReadOnly<string>(["task", "kanban", "move", "status"])),
        new("progress", "Projects & Tasks", "How do I track project progress?", "Project details show completion, milestones, tasks and activity. Keep task status and due dates up to date for the clearest view.", Array.AsReadOnly<string>(["project", "progress", "milestones"])),
        new("plan", "Billing & Invoices", "How do I change my subscription plan?", "Open Pricing, choose the plan that fits your team, and continue with the plan selection flow. Your workspace administrator may need to approve changes.", Array.AsReadOnly<string>(["pricing", "subscription", "upgrade", "plan"])),
        new("invoice", "Billing & Invoices", "How do I create an invoice?", "Go to Invoices, choose Create Invoice, add your customer and line items, then review before saving.", Array.AsReadOnly<string>(["invoice", "billing", "payment"])),
        new("two-factor", "Security & Privacy", "How do I enable two-factor authentication?", "Go to Settings, choose Security, and enable two-factor authentication. Follow the setup steps for your preferred authenticator app.", Array.AsReadOnly<string>(["2fa", "two factor", "security", "authentication"])),
        new("privacy", "Security & Privacy", "How is my account data protected?", "Use the security and privacy controls available to your workspace. For organisation-specific compliance questions, contact your administrator or support team.", Array.AsReadOnly<string>(["privacy", "data", "security"])),
        new("github", "Integrations", "How do I connect GitHub?", "Open Settings, select Connected Apps, then choose GitHub and follow the connection prompts.", Array.AsReadOnly<string>(["github", "connect", "integration"])),
        new("sync", "Integrations", "Why is an integration not syncing?", "Check that the connection is still authorised, then review the connected app settings. Reconnect the app if access was changed.", Array.AsReadOnly<string>(["sync", "integration", "connection"])),
        new("email-alerts", "Notifications", "How do I turn off email notifications?", "Open Settings, choose Notifications, then turn off the update types you no longer want to receive.", Array.AsReadOnly<string>(["email", "notifications", "alerts"])),
        new("quiet-hours", "Notifications", "Can I change notification frequency?", "Yes. Notification controls let you adjust which product, team, and digest updates are delivered.", Array.AsReadOnly<string>(["notification", "frequency", "quiet hours"])),
        new("loading", "Troubleshooting", "Why is a page not loading correctly?", "Refresh the page first. If the problem remains, clear saved UI preferences or contact support with the page name and what you were doing.", Array.AsReadOnly<string>(["page", "loading", "error", "refresh"])),
        new("upload", "Troubleshooting", "What should I do if a file upload fails?", "Check your connection and file size, then try again. If it continues, contact support and include the file type and any error shown.", Array.AsReadOnly<string>(["file", "upload", "failed"])),
    ]);
    public static IReadOnlyList<string> Categories { get; } = Array.AsReadOnly(new[] { "All" }.Concat(Articles.Select(a => a.Category).Distinct()).ToArray());
    public static IReadOnlyList<FaqArticle> Filter(string query, string category) => Articles.Where(a =>
        (category == "All" || a.Category == category) &&
        $"{a.Question} {a.Answer} {a.Category} {string.Join(" ", a.Keywords)}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();
}
