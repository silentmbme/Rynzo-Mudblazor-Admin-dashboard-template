namespace Rynzo.Client.Features.Projects.Models;

public sealed record ProjectRecord(
    string Id, string Name, string Client, string Owner, int Progress, decimal Spent,
    decimal Budget, string Due, string Risk, string Status, string Priority,
    string[] Team, string[] Tags, string Tasks);

public sealed record TeamMember(
    string Name, string Role, string Department, int Utilization, string Availability,
    int ActiveTasks, string[] Projects, string[] Skills, string Email);

public sealed record ProjectMilestone(string Title, string Due, string Status, string Note);
