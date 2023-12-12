namespace NetworkEditor.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static TEnum ToFlags<TEnum>(this Dictionary<string, bool> dictionary)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException("The generic type parameter must be an enum with the [Flags] attribute.");
            }

            int result = 0;
            foreach (var item in dictionary)
            {
                if (item.Value && Enum.TryParse<TEnum>(item.Key, out var enumValue))
                {
                    result |= Convert.ToInt32(enumValue);
                }
            }

            return (TEnum)Enum.ToObject(enumType, result);
        }
    }
}
