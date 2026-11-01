using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
namespace Rynzo.Client.Utilities;
/// <summary>String Number() semantics used by the original local forms; no wire conversion.</summary>
public static class JavaScriptNumber
{
    private static readonly char[] WhiteSpace = ['\u0009', '\u000a', '\u000b', '\u000c', '\u000d', '\u0020', '\u00a0', '\u1680', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u2028', '\u2029', '\u202f', '\u205f', '\u3000', '\ufeff'];
    public static double Parse(string value)
    {
        var text = value.Trim(WhiteSpace);
        if (text.Length == 0) return 0;
        if (text is "Infinity" or "+Infinity") return double.PositiveInfinity;
        if (text == "-Infinity") return double.NegativeInfinity;
        var radix = text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 16 : text.StartsWith("0b", StringComparison.OrdinalIgnoreCase) ? 2 : text.StartsWith("0o", StringComparison.OrdinalIgnoreCase) ? 8 : 0;
        if (radix != 0)
        {
            if (text.Length == 2) return double.NaN;
            var integer = BigInteger.Zero; var overflow = false;
            foreach (var c in text.AsSpan(2))
            {
                var digit = c is >= '0' and <= '9' ? c - '0' : c is >= 'a' and <= 'f' ? c - 'a' + 10 : c is >= 'A' and <= 'F' ? c - 'A' + 10 : -1;
                if (digit < 0 || digit >= radix) return double.NaN;
                if (!overflow) { integer = integer * radix + digit; overflow = integer.GetBitLength() > 1076; }
            }
            if (overflow) return double.PositiveInfinity;
            var bits = (int)integer.GetBitLength();
            if (bits <= 53) return (double)(ulong)integer;
            var shift = bits - 53; var leading = integer >> shift; var remainder = integer - (leading << shift); var half = BigInteger.One << (shift - 1);
            if (remainder > half || remainder == half && !leading.IsEven) leading++;
            return Math.ScaleB((double)(ulong)leading, shift);
        }
        if (!Regex.IsMatch(text, @"^[+-]?(?:[0-9]+(?:\.[0-9]*)?|\.[0-9]+)(?:[eE][+-]?[0-9]+)?$", RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)) return double.NaN;
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ? number : double.NaN;
    }
    public static double OrZero(string value) { var number = Parse(value); return double.IsNaN(number) || number == 0 ? 0 : number; }
}
