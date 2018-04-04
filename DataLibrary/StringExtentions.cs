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

        public static string FillTo(this string str, string fill, int count)
        {
            while (str.Length < count)
            {
                str += fill;
            }

            return str;
        }

        public static string FitTo(this string str, int count)
        {
            if (str.Length > count)
            {
                return str.Substring(0, count - 3) + "...";
            }

            return FillTo(str, " ", count);
        }
    }
}