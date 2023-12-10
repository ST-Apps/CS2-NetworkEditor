namespace NetworkEditor.Extensions
{
    using System;
    using System.Collections.Generic;

    internal static class EnumExtensions
    {
        public static Dictionary<string, bool> FlagsToDictionary<TEnum>(this TEnum enumValue)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException("The generic type parameter must be an enum with the [Flags] attribute.");
            }

            var result = new Dictionary<string, bool>();
            foreach (var value in Enum.GetValues(enumType))
            {
                var enumName = Enum.GetName(enumType, value);
                if (enumName != null)
                {
                    result[enumName] = enumValue.HasFlag((Enum)value);
                }
            }

            return result;
        }
    }
}
