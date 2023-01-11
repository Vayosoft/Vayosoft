using System.Text.RegularExpressions;

namespace Vayosoft.Commons.ValueObjects
{
    public partial record PhoneNumber   
    {
        public const string IsraelCountryPrefix = "972";

        [GeneratedRegex("^[\\d]{5,21}$", RegexOptions.Compiled, "en-US")]
        private static partial Regex Pattern();

        public string Value { get; init; }

        public PhoneNumber(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException($"{nameof(value)} needs to be defined as valid phone number.");

            Value = value;
        }
        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber?.Value;
        public static explicit operator PhoneNumber(string value) => new(value);

        public static bool IsValid(string value) => Pattern().IsMatch(value);

        public string LocalPhoneNumber => RemoveIsraelCountryPrefix(Value);

        public static string RemoveIsraelCountryPrefix(string phone)
        {
            if (phone.IndexOf('+' + IsraelCountryPrefix, StringComparison.Ordinal) == 0)
                return "0" + phone[4..];

            if (phone.IndexOf(IsraelCountryPrefix, StringComparison.Ordinal) == 0)
                return "0" + phone[3..];

            return phone;
        }
    }
}