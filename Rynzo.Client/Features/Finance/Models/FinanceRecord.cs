namespace Rynzo.Client.Features.Finance.Models;

public sealed record FinanceRecord(string Id, string Name, string Category, decimal Amount, decimal Used, string Status, DateOnly Date);

public static class FinanceSource
{
    public static readonly FinanceRecord[] Transactions =
    [
        new("TX-1001", "Northstar payment", "Income", 8400, 0, "Completed", new(2026,10,9)),
        new("TX-1002", "Office rent", "Expense", 2200, 0, "Completed", new(2026,10,8)),
        new("TX-1003", "Software subscriptions", "Expense", 450, 0, "Pending", new(2026,10,8)),
        new("TX-1004", "Atlas consulting payment", "Income", 5200, 0, "Completed", new(2026,10,7)),
        new("TX-1005", "Team travel", "Expense", 620, 0, "Completed", new(2026,10,7)),
        new("TX-1006", "Cedar design deposit", "Income", 2100, 0, "Pending", new(2026,10,6)),
        new("TX-1007", "Equipment purchase", "Expense", 1800, 0, "Completed", new(2026,10,5)),
        new("TX-1008", "Summit subscription renewal", "Income", 3400, 0, "Completed", new(2026,10,4)),
        new("TX-1009", "Marketing campaign", "Expense", 1250, 0, "Pending", new(2026,10,3)),
        new("TX-1010", "Greenfield service payment", "Income", 4600, 0, "Completed", new(2026,10,2)),
        new("TX-1011", "Internet and utilities", "Expense", 380, 0, "Completed", new(2026,10,1)),
        new("TX-1012", "Office maintenance", "Expense", 720, 0, "Completed", new(2026,9,30))
    ];

    public static readonly FinanceRecord[] Invoices =
    [
        new("INV-1001", "Northstar Retail", "Services", 8400, 0, "Paid", new(2026,10,20)),
        new("INV-1002", "Atlas Supplies", "Consulting", 3600, 0, "Unpaid", new(2026,10,22)),
        new("INV-1003", "Cedar Trading", "Design", 2100, 0, "Unpaid", new(2026,10,25)),
        new("INV-1004", "Summit Electronics", "Development", 6200, 0, "Unpaid", new(2026,10,5)),
        new("INV-1005", "Greenfield Foods", "Consulting", 2800, 0, "Paid", new(2026,10,2)),
        new("INV-1006", "Urban Living", "Design", 4500, 0, "Unpaid", new(2026,10,15)),
        new("INV-1007", "Bluebird Studio", "Services", 1800, 0, "Unpaid", new(2026,10,7)),
        new("INV-1008", "Westbridge Partners", "Consulting", 7200, 0, "Paid", new(2026,10,1))
    ];

    public static readonly FinanceRecord[] Expenses =
    [
        new("EXP-1001", "Team travel", "Travel", 620, 0, "Pending", new(2026,10,9)),
        new("EXP-1002", "Office supplies", "Office", 280, 0, "Approved", new(2026,10,8)),
        new("EXP-1003", "Cloud hosting", "Software", 450, 0, "Pending", new(2026,10,7)),
        new("EXP-1004", "Client lunch", "Meals", 145, 0, "Pending", new(2026,10,9)),
        new("EXP-1005", "Design workshop", "Training", 850, 0, "Approved", new(2026,10,9)),
        new("EXP-1006", "Security software", "Software", 360, 0, "Pending", new(2026,10,9)),
        new("EXP-1007", "Conference tickets", "Travel", 1200, 0, "Rejected", new(2026,10,9))
    ];

    public static readonly FinanceRecord[] Budgets =
    [
        new("BDG-1001", "Marketing budget", "Marketing", 10000, 6200, "Active", new(2026,10,31)),
        new("BDG-1002", "Engineering budget", "Engineering", 18000, 12400, "Active", new(2026,10,31)),
        new("BDG-1003", "Operations budget", "Operations", 8000, 5300, "Active", new(2026,10,31)),
        new("BDG-1004", "Customer support budget", "Support", 6000, 4900, "Active", new(2026,10,31)),
        new("BDG-1005", "Training budget", "Training", 4500, 4100, "Active", new(2026,10,31)),
        new("BDG-1006", "Office budget", "Office", 9000, 2800, "Active", new(2026,10,31))
    ];
}
