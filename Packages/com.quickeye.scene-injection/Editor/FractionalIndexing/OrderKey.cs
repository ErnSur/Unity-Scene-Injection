using System;
using System.Linq;

namespace QuickEye.SceneInjection.FractionalIndexing
{
    public class OrderKey
    {
        public const string Base62Digits =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        
        public static string Midpoint(string a, string b, string digits)
        {
            var zero = digits[0].ToString();
            if (b != null && string.Compare(a, b, StringComparison.Ordinal) >= 0)
            {
                throw new InvalidOperationException($"{a} >= {b}");
            }
         
            if (a == zero || (b != null && b == zero))
            {
                throw new InvalidOperationException("trailing zero");
            }

            if (b != null)
            {
                var n = 0;
                while (n < b.Length && (b[n] == zero[0] || (n < a.Length && a[n] == b[n])))
                {
                    n++;
                }

                if (n > 0)
                {
                    var newA = n > a.Length ? "" : a.Substring(n);
                    return b.Substring(0, n) + Midpoint(newA, b.Substring(n), digits);
                }
            }

            var digitA = a?.Length > 0 ? digits.IndexOf(a[0]) : 0;
            var digitB = b != null ? digits.IndexOf(b[0]) : digits.Length;
            if (digitB - digitA > 1)
            {
                var midDigit = (int)Math.Round(0.5 * (digitA + digitB));
                
                return digits[midDigit].ToString();
            }
            else
            {
                if (b != null && b.Length > 1)
                {
                    return b.Substring(0, 1);
                }
                else
                {
                    var newA = a.Length < 1 ? "" : a.Substring(1);
                    return digits[digitA] + Midpoint(newA, null, digits);
                }
            }
        }
        
        public static string GenerateKeyBetween(string a, string b
            , string digits = Base62Digits)
        {
            if (a != null)
            {
                ValidateOrderKey(a, digits);
            }

            if (b != null)
            {
                ValidateOrderKey(b, digits);
            }

            if (a != null && b != null && string.CompareOrdinal(a, b) >= 0)
            {
                throw new InvalidOperationException($"{a} >= {b}");
            }

            string ib;
            string fb;
            if (a == null)
            {
                if (b == null)
                {
                    return "a" + digits[0];
                }

                ib = GetIntegerPart(b);
                fb = b.Substring(ib.Length);
                if (ib == "A" + new string(digits[0], 26))
                {
                    return ib + Midpoint("", fb, digits);
                }

                if (string.CompareOrdinal(ib, b) < 0)
                {
                    return ib;
                }

                var res = DecrementInteger(ib, digits);
                if (res == null)
                {
                    throw new InvalidOperationException("cannot decrement any more");
                }

                return res;
            }

            string ia;
            string fa;
            string i;
            if (b == null)
            {
                ia = GetIntegerPart(a);
                fa = a.Substring(ia.Length);
                i = IncrementInteger(ia, digits);
                return i == null ? ia + Midpoint(fa, null, digits) : i;
            }

            ia = GetIntegerPart(a);
            fa = a.Substring(ia.Length);
            ib = GetIntegerPart(b);
            fb = b.Substring(ib.Length);
            if (ia == ib)
            {
                return ia + Midpoint(fa, fb, digits);
            }

            i = IncrementInteger(ia, digits);
            if (i == null)
            {
                throw new InvalidOperationException("cannot increment any more");
            }

            if (string.CompareOrdinal(i, b) < 0)
            {
                return i;
            }

            return ia + Midpoint(fa, null, digits);
        }

        private static void ValidateInteger(string integer)
        {
            if (integer.Length != GetIntegerLength(integer[0]))
            {
                throw new InvalidOperationException("invalid integer part of order key: " + integer);
            }
        }

        private static int GetIntegerLength(char head)
        {
            if (head >= 'a' && head <= 'z')
            {
                return head - 'a' + 2;
            }
            else if (head >= 'A' && head <= 'Z')
            {
                return 'Z' - head + 2;
            }
            else
            {
                throw new InvalidOperationException("invalid order key head: " + head);
            }
        }

        private static string GetIntegerPart(string key)
        {
            var integerPartLength = GetIntegerLength(key[0]);
            if (integerPartLength > key.Length)
            {
                throw new InvalidOperationException("invalid order key: " + key);
            }

            return key.Substring(0, integerPartLength);
        }

        private static void ValidateOrderKey(string key, string digits)
        {
            if (key == "A" + new string(digits[0], 26))
            {
                throw new InvalidOperationException("invalid order key: " + key);
            }

            var i = GetIntegerPart(key);
            var f = key.Substring(i.Length);
            if (f.EndsWith(digits[0].ToString()))
            {
                throw new InvalidOperationException("invalid order key: " + key);
            }
        }

        private static string IncrementInteger(string x, string digits)
        {
            ValidateInteger(x);
            var head = x[0];
            var digs = x.Substring(1).ToCharArray();
            var carry = true;
            for (var i = digs.Length - 1; carry && i >= 0; i--)
            {
                var d = digits.IndexOf(digs[i]) + 1;
                if (d == digits.Length)
                {
                    digs[i] = digits[0];
                }
                else
                {
                    digs[i] = digits[d];
                    carry = false;
                }
            }

            if (carry)
            {
                if (head == 'Z')
                {
                    return "a" + digits[0];
                }

                if (head == 'z')
                {
                    return null;
                }

                var h = (char)(head + 1);
                if (h > 'a')
                {
                    digs = digs.Append(digits[0]).ToArray();
                }
                else
                {
                    Array.Resize(ref digs, digs.Length - 1);
                }

                return h + new string(digs);
            }
            else
            {
                return head + new string(digs);
            }
        }

        private static string DecrementInteger(string x, string digits)
        {
            ValidateInteger(x);
            var head = x[0];
            var digs = x.Substring(1).ToCharArray();
            var borrow = true;
            for (var i = digs.Length - 1; borrow && i >= 0; i--)
            {
                var d = digits.IndexOf(digs[i]) - 1;
                if (d == -1)
                {
                    digs[i] = digits[digits.Length - 1];
                }
                else
                {
                    digs[i] = digits[d];
                    borrow = false;
                }
            }

            if (borrow)
            {
                if (head == 'a')
                {
                    return "Z" + digits[digits.Length - 1];
                }

                if (head == 'A')
                {
                    return null;
                }

                var h = (char)(head - 1);
                if (h < 'Z')
                {
                    digs = digs.Append(digits.Last()).ToArray();
                }
                else
                {
                    Array.Resize(ref digs, digs.Length - 1);
                }

                return h + new string(digs);
            }
            else
            {
                return head + new string(digs);
            }
        }
    }
}