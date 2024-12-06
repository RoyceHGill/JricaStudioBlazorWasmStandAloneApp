using System.Text;

namespace JricaStudioApp.Extensions
{
    public static class StringCaseConverter
    {
        public static string ToFlatCase(this string value)
        {
            return value.ToLower();
        }

        public static string FromPascalToString(this string value)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                var character = value[i];
                if (char.IsAsciiLetterUpper(character) && i != 0)
                {
                    stringBuilder.Append(' ');
                    stringBuilder.Append(character);
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
