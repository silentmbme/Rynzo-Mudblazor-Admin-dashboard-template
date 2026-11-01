using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record HospitalDepartment(string Name, int Beds, int Occupied, int Visits, string Color);
public sealed record HospitalDoctor(string Id, string Name, string Initials, string Image, string Department, string Role, int Patients, bool Available, string Tone);
public sealed record HospitalAppointment(string Id, string Patient, int? Age, string DoctorId, string Department, string Date, string Time, string Type, string Status);
public sealed record HospitalSupply(string Name, int Remaining, int Target, string Unit);
public sealed record HospitalActivity(string Title, string Text, string Time, string Icon, string Tone);
public sealed record HospitalOverview(HospitalDepartment[] Departments, int Visits, int Beds, int Occupied, int Available, double Occupancy, int[] Admissions, int[] Discharged, string[] Labels, int Revenue);
public sealed record HospitalSourceData(HospitalDepartment[] Departments, HospitalDoctor[] Doctors, HospitalAppointment[] Appointments, HospitalSupply[] Supplies, HospitalActivity[] Activities)
{
    public static HospitalSourceData Load() { using var stream = typeof(HospitalSourceData).Assembly.GetManifestResourceStream("Rynzo.HospitalSource") ?? throw new InvalidOperationException("Hospital source unavailable."); return JsonSerializer.Deserialize<HospitalSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Hospital source invalid."); }
}
public sealed class HospitalBooking
{
    public string Patient { get; set; } = "";
    public string DoctorId { get; set; } = "dr-01";
    public string Date { get; set; } = "2026-10-09";
    public string Time { get; set; } = "14:00";
    public string Type { get; set; } = "Consultation";
}
