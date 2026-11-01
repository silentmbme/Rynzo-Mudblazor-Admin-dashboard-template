namespace Rynzo.Client.Features.Crypto.Models;

public sealed record CryptoActivity(string Id, string Asset, string Type, double Amount, string Status, string Date);
