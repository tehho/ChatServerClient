namespace System
{
    public static class StringExtentions
    {
        public static string ToCapitalize(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            string temp = "";

            temp = str.Substring(0, 1).ToUpper();
            temp = str.Substring(1).ToLower();

            return temp;
        }
    }
}