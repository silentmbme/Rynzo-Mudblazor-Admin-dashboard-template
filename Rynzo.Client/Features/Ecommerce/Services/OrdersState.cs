using Rynzo.Client.Features.Ecommerce.Models;
namespace Rynzo.Client.Features.Ecommerce.Services;

public sealed class OrdersState
{
    public OrdersSource Source { get; } = OrdersSource.Load();
    public List<StoreOrder> Orders { get; }
    public List<string> Selected { get; } = [];
    public StoreOrder? Active { get; set; }
    public string Query { get; set; } = "";
    public string Status { get; set; } = "All statuses";
    public string Payment { get; set; } = "All payments";
    public string Fulfillment { get; set; } = "All fulfillment";
    public string Channel { get; set; } = "All channels";
    public OrdersState() => Orders = [.. Source.Orders];
    public StoreOrder[] Shown { get { var term = Query.Trim(); return Orders.Where(o => (term.Length == 0 || new[] { o.Id, o.Customer, o.Email }.Any(s => s.Contains(term, StringComparison.OrdinalIgnoreCase))) && (Status == "All statuses" || o.Status == Status) && (Payment == "All payments" || o.Payment == Payment) && (Fulfillment == "All fulfillment" || o.Fulfillment == Fulfillment) && (Channel == "All channels" || o.Channel == Channel)).ToArray(); } }
    public bool SelectedAll => Shown.Length > 0 && Shown.All(o => Selected.Contains(o.Id));
    public void Toggle(string id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void SelectAll() { var clear = SelectedAll; Selected.Clear(); if (!clear) Selected.AddRange(Shown.Select(o => o.Id)); }
    public void Reset() { Query = ""; Status = "All statuses"; Payment = "All payments"; Fulfillment = "All fulfillment"; Channel = "All channels"; }
    public void ProcessSelected() { for (var i = 0; i < Orders.Count; i++) if (Selected.Contains(Orders[i].Id)) Orders[i] = Orders[i] with { Status = "Processing" }; }
    public void Apply(string action, string next) { if (Active is null) return; StoreOrder Patch(StoreOrder o) => action switch { "status" => o with { Status = next }, "payment" => o with { Payment = next }, "refund" => o with { Payment = "Refunded", Status = "Returned", Fulfillment = "Returned" }, "cancel" => o with { Status = "Cancelled" }, _ => o }; var id = Active.Id; for (var i = 0; i < Orders.Count; i++) if (Orders[i].Id == id) Orders[i] = Patch(Orders[i]); Active = Patch(Active); }
    public string Export() => "Order,Customer,Items,Total,Payment,Fulfillment,Status,Channel" + string.Concat(Shown.Select(o => "\n" + string.Join(',', o.Id, o.Customer, o.Items.ToString(System.Globalization.CultureInfo.InvariantCulture), o.Total.ToString("0.############################", System.Globalization.CultureInfo.InvariantCulture), o.Payment, o.Fulfillment, o.Status, o.Channel)));
    public static string Portrait(string name) => Rynzo.Client.Assets.SourceUserImages.Get(name);
}
