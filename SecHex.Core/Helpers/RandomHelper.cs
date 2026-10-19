using System.Text;

namespace SecHex.Core.Helpers
{
    public static class RandomHelper
    {
        private const string Alphanumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string HexDigits = "ABCDEF0123456789";
        private const string ValidSecondMacNibble = "26AE"; // Garante bit de endereço local administrado (LAA)

        public static string GenerateAlphanumeric(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(Alphanumeric[Random.Shared.Next(Alphanumeric.Length)]);
            }
            return sb.ToString();
        }

        public static string GenerateMacAddress(char separator = '-')
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(HexDigits[Random.Shared.Next(HexDigits.Length)]);
            sb.Append(ValidSecondMacNibble[Random.Shared.Next(ValidSecondMacNibble.Length)]);

            for (int i = 0; i < 5; i++)
            {
                if (separator != '\0')
                {
                    sb.Append(separator);
                }
                sb.Append(HexDigits[Random.Shared.Next(HexDigits.Length)]);
                sb.Append(HexDigits[Random.Shared.Next(HexDigits.Length)]);
            }

            return sb.ToString();
        }

        public static string GenerateRandomDateString(int minYear = 2018, int maxYear = 2024)
        {
            int year = Random.Shared.Next(minYear, maxYear + 1);
            int month = Random.Shared.Next(1, 13);
            int day = Random.Shared.Next(1, 29);
            return $"{day:D2}/{month:D2}/{year}";
        }
    }
}
