namespace CSV {
    
    using System;
    using System.ComponentModel;
    public enum Delimiter
    {
        Auto,
        Comma,
        Tab
    }

    public static class DelimiterExtensions
    {
        public static char ToChar(this Delimiter delimiter)
        {
            switch (delimiter)
            {
                case Delimiter.Auto:
                    throw new InvalidEnumArgumentException("Could not return char of Delimiter.Auto.");
                case Delimiter.Comma:
                    return ',';
                case Delimiter.Tab:
                    return '\t';
                default:
                    throw new ArgumentOutOfRangeException(nameof(delimiter), delimiter, null);
            }
        }
    }
}