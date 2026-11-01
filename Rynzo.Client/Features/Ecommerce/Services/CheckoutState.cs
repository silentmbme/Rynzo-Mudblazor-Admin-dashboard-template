using Rynzo.Client.Features.Ecommerce.Models;
using System.Text.RegularExpressions;
namespace Rynzo.Client.Features.Ecommerce.Services;

public sealed class CheckoutState
{
    public CheckoutSource Source { get; } = CheckoutSource.Load();
    public List<CheckoutItem> Items { get; }
    public CheckoutContact Contact { get; }
    public CheckoutAddress Shipping { get; }
    public CheckoutAddress Billing { get; }
    public CheckoutCard Card { get; }
    public bool SameBilling { get; set; } = true;
    public bool SaveAddress { get; set; } = true;
    public string Delivery { get; set; } = "Standard";
    public string Payment { get; set; } = "Card";
    public string Coupon { get; set; } = "";
    public string AppliedCoupon { get; set; } = "";
    public string Notes { get; set; } = "";
    public string[] Errors { get; private set; } = [];
    public string? Success { get; set; }
    public CheckoutState() { Items = [.. Source.BaseItems]; Contact = Source.InitialContact with { }; Shipping = Source.InitialAddress with { }; Billing = Source.InitialAddress with { }; Card = Source.DemoCard with { }; }
    public CheckoutTotals Totals { get { var subtotal = Items.Sum(i => i.Price * i.Quantity); var shipping = AppliedCoupon == "FREESHIP" ? 0 : Source.DeliveryInfo[Delivery].Price; var discount = AppliedCoupon == "SAVE10" ? subtotal * .1 : AppliedCoupon == "WELCOME15" ? subtotal * .15 : 0; var tax = (subtotal - discount + shipping) * .08; return new(subtotal, shipping, discount, tax, subtotal - discount + shipping + tax); } }
    public void Quantity(int id, int delta) { var index = Items.FindIndex(i => i.Id == id); if (index >= 0) Items[index] = Items[index] with { Quantity = Math.Max(1, Items[index].Quantity + delta) }; }
    public bool ApplyCoupon() { var code = Coupon.Trim().ToUpperInvariant(); if (code is not ("SAVE10" or "WELCOME15" or "FREESHIP")) return false; AppliedCoupon = code; return true; }
    public string CardNumber { get => Card.Number; set => Card.Number = Regex.Replace(value, "[^0-9 ]", ""); }
    public bool Place() { var errors = new List<string>(); if (Contact.First.Length == 0 || Contact.Last.Length == 0) errors.Add("Enter first and last name."); if (!Regex.IsMatch(Contact.Email, @"^\S+@\S+\.\S+$")) errors.Add("Enter a valid email address."); if (Contact.Phone.Length > 0 && Regex.Replace(Contact.Phone, "[^0-9]", "").Length < 7) errors.Add("Enter a valid phone number."); if (Shipping.Line1.Length == 0 || Shipping.City.Length == 0 || Shipping.Postal.Length == 0) errors.Add("Complete your shipping address."); if (Payment == "Card" && (Card.Holder.Length == 0 || Regex.Replace(Card.Number, @"\s", "").Length < 12 || !Regex.IsMatch(Card.Expiry, @"^[0-9]{2}/[0-9]{2}$") || Card.Cvv.Length < 3)) errors.Add("Complete valid card details."); if (Items.Count == 0) errors.Add("Your cart is empty."); Errors = errors.ToArray(); if (Errors.Length > 0) return false; Success = "#ORD-" + Random.Shared.Next(10500, 10900); return true; }
}
