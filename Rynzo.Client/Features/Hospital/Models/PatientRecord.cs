namespace Rynzo.Client.Features.Hospital.Models;

public sealed record PatientRecord(string Id, string Name, int Age, string Contact, string Department, string Status, string Image);

public sealed class PatientDirectoryState
{
    public List<PatientRecord> Patients { get; } =
    [
        new("PAT-101", "Olivia Bennett", 34, "+1 202-555-0110", "General Medicine", "Inpatient", "assets/images/users/user-1.jpg"),
        new("PAT-102", "Liam Carter", 58, "+1 202-555-0111", "Cardiology", "Outpatient", "assets/images/users/user-2.jpg"),
        new("PAT-103", "Emma Davis", 8, "+1 202-555-0112", "Pediatrics", "Outpatient", "assets/images/users/user-3.jpg"),
        new("PAT-104", "Noah Williams", 42, "+1 202-555-0113", "Orthopedics", "Inpatient", "assets/images/users/user-4.jpg"),
        new("PAT-105", "Sophia Miller", 29, "+1 202-555-0114", "General Medicine", "Outpatient", "assets/images/users/user-1.jpg"),
        new("PAT-106", "Ethan Reed", 63, "+1 202-555-0115", "Cardiology", "Outpatient", "assets/images/users/user-2.jpg")
    ];

    public string? Add(string name, int age, string contact, string department)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(contact)) return "Enter the patient name and contact.";
        if (age is < 0 or > 120) return "Age must be a whole number from 0 to 120.";
        var id = $"PAT-{Patients.Max(patient => int.Parse(patient.Id[4..])) + 1}";
        Patients.Add(new PatientRecord(id, name.Trim(), age, contact.Trim(), department, "Outpatient", "assets/images/users/user-1.jpg"));
        return null;
    }

    public string Csv(IEnumerable<PatientRecord> patients)
    {
        var rows = new List<string> { "Reference,Name,Category,Status,Age,Contact" };
        rows.AddRange(patients.Select(patient => string.Join(',', patient.Id, Quote(patient.Name), Quote(patient.Department), patient.Status, patient.Age, Quote(patient.Contact))));
        return string.Join("\r\n", rows);
    }

    private static string Quote(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
