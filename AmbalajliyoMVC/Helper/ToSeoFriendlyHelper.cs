namespace AmbalajliyoMVC.Helper
{
    public static class ToSeoFriendlyHelper
    {
        public static string ToSeoFriendly(this string text)
        {
            text = text.ToLower().Replace(" ", "-").Replace("ç", "c").Replace("ş", "s")
                .Replace("ğ", "g").Replace("ü", "u").Replace("ö", "o").Replace("ı", "i")
                .Replace("'", "").Replace("\"", "").Replace("?", "").Replace("&", "")
                .Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "")
                .Replace(".", "").Replace(",", "").Replace("+", "").Replace("=", "")
                .Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "")
                .Replace("{", "").Replace("}", "").Replace("|", "").Replace("^", "")
                .Replace("!", "").Replace("%", "").Replace("*", "").Replace("#", "")
                .Replace("@", "").Replace("$", "").Replace("~", "").Replace("`", "");
            return text;
        }
    }
}
