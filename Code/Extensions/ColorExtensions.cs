namespace NetworkEditor.Code.Extensions
{
    using UnityEngine;

    internal static class ColorExtensions
    {
        public static Color WithTransparency(this Color color, float transparency)
        {
            return new Color(color.r, color.g, color.b, transparency);
        }
    }
}
