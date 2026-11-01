using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class HospitalDashboardState
{
    public HospitalSourceData Source { get; } = HospitalSourceData.Load();
    public List<HospitalAppointment> Appointments { get; }
    public HospitalBooking Booking { get; } = new();
    public HospitalDashboardState() => Appointments = [.. Source.Appointments];
    private static int Round(double value) => (int)Math.Floor(value + .5);
    public HospitalOverview Overview(int period, string department)
    {
        if (period is not (7 or 30)) throw new ArgumentOutOfRangeException(nameof(period));
        var departments = Source.Departments.Where(d => department == "all" || d.Name == department).ToArray(); var visits = departments.Sum(d => Round(d.Visits * period / 7d)); var beds = departments.Sum(d => d.Beds); var occupied = departments.Sum(d => d.Occupied);
        double[] weights = [.8, .92, 1.06, .94, 1.24, .9, 1.14]; var total = weights.Sum(); var admissions = weights.Select(w => (int)Math.Floor(visits * w / total)).ToArray(); admissions[^1] += visits - admissions.Sum(); var discharged = admissions.Select((v, i) => Round(v * (.73 + i % 3 * .05))).ToArray();
        var labels = Enumerable.Range(0, 7).Select(i => new DateTime(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc).AddDays(-Round((6 - i) * (period - 1d) / 6)).ToString("MMM d", CultureInfo.GetCultureInfo("en-US"))).ToArray();
        return new(departments, visits, beds, occupied, beds - occupied, beds == 0 ? 0 : occupied / (double)beds * 100, admissions, discharged, labels, visits * 148);
    }
    public HospitalAppointment[] DepartmentAppointments(string department) => Appointments.Where(a => department == "all" || a.Department == department).ToArray();
    public HospitalAppointment[] Filter(string department, string status, string query) => DepartmentAppointments(department).Where(a => (status == "all" || a.Status == status) && $"{a.Patient} {a.Id} {a.Department}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();
    public void OpenBooking(string doctorId = "dr-01") { Booking.Patient = ""; Booking.DoctorId = doctorId; Booking.Date = "2026-10-09"; Booking.Time = "14:00"; }
    public const string ValidationError = "Enter a patient name, an available doctor, and a valid appointment date and time.";
    public const string DuplicateError = "This doctor already has an appointment at that time. Choose another time.";
    public string? Book()
    {
        var b = Booking; var doctor = Source.Doctors.SingleOrDefault(d => d.Id == b.DoctorId && d.Available);
        if (doctor is null || b.Patient.Trim().Length < 2 || b.Patient.Length > 80 || !DateOnly.TryParseExact(b.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) || date < new DateOnly(2026, 10, 9) || !TimeOnly.TryParseExact(b.Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) || b.Type is not ("Consultation" or "Follow-up")) return ValidationError;
        if (Appointments.Any(a => a.DoctorId == b.DoctorId && a.Date == b.Date && a.Time == b.Time)) return DuplicateError;
        Appointments.Insert(0, new($"APT-{2101 + Appointments.Count}", b.Patient.Trim(), null, doctor.Id, doctor.Department, b.Date, b.Time, b.Type, "Scheduled")); return null;
    }
    public bool CheckIn(string id) { var index = Appointments.FindIndex(a => a.Id == id && a.Status == "Scheduled"); if (index < 0) return false; Appointments[index] = Appointments[index] with { Status = "Checked in" }; return true; }
    public string Csv(IEnumerable<HospitalAppointment> appointments) { var rows = new List<string[]> { new[] { "Appointment", "Patient", "Department", "Doctor", "Date", "Time", "Status" } }; rows.AddRange(appointments.Select(a => new[] { a.Id, a.Patient, a.Department, Source.Doctors.Single(d => d.Id == a.DoctorId).Name, a.Date, a.Time, a.Status })); return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\"")))); }
}
