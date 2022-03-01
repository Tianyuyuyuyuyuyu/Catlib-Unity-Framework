using Microsoft.SqlServer.Server;
using UnityEngine;

namespace Framework.Utility.Editor
{
    /// <summary>GUI extension methods</summary>
    public static class GUIStyleExtensions
    {
        /// <summary>
        /// Clones the style and adds the given formats to it. You can pass any of these types of values:
        /// <list type="bullet">
        /// <item><term>Format:</term><description>Rich-text, wordwrap</description></item>
        /// <item><term>FontStyle:</term><description>Font style</description></item>
        /// <item><term>TextAnchor:</term><description>Content anchor</description></item>
        /// <item><term>int:</term><description>Font size</description></item>
        /// <item><term>Color/DeSkinColor:</term><description>Font color</description></item>
        /// </list>
        /// </summary>
        public static GUIStyle Clone(this GUIStyle style, params object[] formats) => new GUIStyle(style).Add(formats);

        /// <summary>
        /// Adds the given formats to the style. You can pass any of these types of values:
        /// <list type="bullet">
        /// <item><term>Format:</term><description>RichText, WordWrap</description></item>
        /// <item><term>FontStyle:</term><description>Font style</description></item>
        /// <item><term>TextAnchor:</term><description>Content anchor</description></item>
        /// <item><term>int:</term><description>Font size</description></item>
        /// <item><term>Color/DeSkinColor:</term><description>Font color</description></item>
        /// </list>
        /// </summary>
        public static GUIStyle Add(this GUIStyle style, params object[] formats)
        {
            foreach (object format in formats)
            {
                System.Type type = format.GetType();
                if (type == typeof(Format))
                {
                    switch ((Format) format)
                    {
                        case Format.RichText:
                            style.richText = true;
                            continue;
                        case Format.WordWrap:
                            style.wordWrap = true;
                            continue;
                        case Format.NoRichText:
                            style.richText = false;
                            continue;
                        case Format.NoWordWrap:
                            style.wordWrap = false;
                            continue;
                        default:
                            continue;
                    }
                }
                else if (type == typeof(FontStyle))
                    style.fontStyle = (FontStyle) format;
                else if (type == typeof(TextAnchor))
                    style.alignment = (TextAnchor) format;
                else if (type == typeof(int))
                    style.fontSize = (int) format;
                else if (type == typeof(Color))
                {
                    GUIStyleState normal = style.normal;
                    GUIStyleState onNormal = style.onNormal;
                    GUIStyleState active = style.active;
                    GUIStyleState onActive = style.onActive;
                    GUIStyleState focused = style.focused;
                    GUIStyleState onFocused = style.onFocused;
                    GUIStyleState hover = style.hover;
                    Color color1;
                    style.onHover.textColor = color1 = (Color) format;
                    Color color2;
                    Color color3 = color2 = color1;
                    hover.textColor = color2;
                    Color color4;
                    Color color5 = color4 = color3;
                    onFocused.textColor = color4;
                    Color color6;
                    Color color7 = color6 = color5;
                    focused.textColor = color6;
                    Color color8;
                    Color color9 = color8 = color7;
                    onActive.textColor = color8;
                    Color color10;
                    Color color11 = color10 = color9;
                    active.textColor = color10;
                    Color color12;
                    Color color13 = color12 = color11;
                    onNormal.textColor = color12;
                    Color color14 = color13;
                    normal.textColor = color14;
                }
            }

            return style;
        }

        /// <summary>Sets the border of the style</summary>
        public static GUIStyle Border(this GUIStyle style, RectOffset border)
        {
            style.border = border;
            return style;
        }

        /// <summary>Sets the border of the style</summary>
        public static GUIStyle Border(
            this GUIStyle style,
            int left,
            int right,
            int top,
            int bottom)
        {
            style.border = new RectOffset(left, right, top, bottom);
            return style;
        }

        /// <summary>Sets the background of the style</summary>
        public static GUIStyle Background(
            this GUIStyle style,
            Texture2D background,
            Texture2D pressBackground = null)
        {
            if ((UnityEngine.Object) pressBackground == (UnityEngine.Object) null)
                pressBackground = background;
            style.normal.background = style.onNormal.background = style.focused.background = style.onFocused.background = style.hover.background = style.onHover.background = background;
            style.active.background = style.onActive.background = pressBackground;
            return style;
        }

        /// <summary>Sets the contentOffset of the style</summary>
        public static GUIStyle ContentOffset(this GUIStyle style, Vector2 offset)
        {
            style.contentOffset = offset;
            return style;
        }

        /// <summary>Sets the contentOffset of the style</summary>
        public static GUIStyle ContentOffset(this GUIStyle style, float offsetX, float offsetY)
        {
            style.contentOffset = new Vector2(offsetX, offsetY);
            return style;
        }

        /// <summary>Sets the X contentOffset of the style</summary>
        public static GUIStyle ContentOffsetX(this GUIStyle style, float offsetX)
        {
            Vector2 contentOffset = style.contentOffset;
            contentOffset.x = offsetX;
            style.contentOffset = contentOffset;
            return style;
        }

        /// <summary>Sets the Y contentOffset of the style</summary>
        public static GUIStyle ContentOffsetY(this GUIStyle style, float offsetY)
        {
            Vector2 contentOffset = style.contentOffset;
            contentOffset.y = offsetY;
            style.contentOffset = contentOffset;
            return style;
        }

        /// <summary>Sets the margin of the style</summary>
        public static GUIStyle Margin(this GUIStyle style, RectOffset margin)
        {
            style.margin = margin;
            return style;
        }

        /// <summary>Sets the margin of the style</summary>
        public static GUIStyle Margin(
            this GUIStyle style,
            int left,
            int right,
            int top,
            int bottom)
        {
            style.margin = new RectOffset(left, right, top, bottom);
            return style;
        }

        /// <summary>Sets the margin of the style</summary>
        public static GUIStyle Margin(this GUIStyle style, int margin)
        {
            style.margin = new RectOffset(margin, margin, margin, margin);
            return style;
        }

        /// <summary>Sets the left margin of the style</summary>
        public static GUIStyle MarginLeft(this GUIStyle style, int left)
        {
            style.margin.left = left;
            return style;
        }

        /// <summary>Sets the right margin of the style</summary>
        public static GUIStyle MarginRight(this GUIStyle style, int right)
        {
            style.margin.right = right;
            return style;
        }

        /// <summary>Sets the top margin of the style</summary>
        public static GUIStyle MarginTop(this GUIStyle style, int top)
        {
            style.margin.top = top;
            return style;
        }

        /// <summary>Sets the bottom margin of the style</summary>
        public static GUIStyle MarginBottom(this GUIStyle style, int bottom)
        {
            style.margin.bottom = bottom;
            return style;
        }

        /// <summary>Sets the overflow of the style</summary>
        public static GUIStyle Overflow(this GUIStyle style, RectOffset overflow)
        {
            style.overflow = overflow;
            return style;
        }

        /// <summary>Sets the overflow of the style</summary>
        public static GUIStyle Overflow(
            this GUIStyle style,
            int left,
            int right,
            int top,
            int bottom)
        {
            style.overflow = new RectOffset(left, right, top, bottom);
            return style;
        }

        /// <summary>Sets the overflow of the style</summary>
        public static GUIStyle Overflow(this GUIStyle style, int overflow)
        {
            style.overflow = new RectOffset(overflow, overflow, overflow, overflow);
            return style;
        }

        /// <summary>Sets the left overflow of the style</summary>
        public static GUIStyle OverflowLeft(this GUIStyle style, int left)
        {
            style.overflow.left = left;
            return style;
        }

        /// <summary>Sets the right overflow of the style</summary>
        public static GUIStyle OverflowRight(this GUIStyle style, int right)
        {
            style.overflow.right = right;
            return style;
        }

        /// <summary>Sets the top overflow of the style</summary>
        public static GUIStyle OverflowTop(this GUIStyle style, int top)
        {
            style.overflow.top = top;
            return style;
        }

        /// <summary>Sets the bottom overflow of the style</summary>
        public static GUIStyle OverflowBottom(this GUIStyle style, int bottom)
        {
            style.overflow.bottom = bottom;
            return style;
        }

        /// <summary>Sets the padding of the style</summary>
        public static GUIStyle Padding(this GUIStyle style, RectOffset padding)
        {
            style.padding = padding;
            return style;
        }

        /// <summary>Sets the padding of the style</summary>
        public static GUIStyle Padding(
            this GUIStyle style,
            int left,
            int right,
            int top,
            int bottom)
        {
            style.padding = new RectOffset(left, right, top, bottom);
            return style;
        }

        /// <summary>Sets the padding of the style</summary>
        public static GUIStyle Padding(this GUIStyle style, int padding)
        {
            style.padding = new RectOffset(padding, padding, padding, padding);
            return style;
        }

        /// <summary>Sets the left padding of the style</summary>
        public static GUIStyle PaddingLeft(this GUIStyle style, int left)
        {
            style.padding.left = left;
            return style;
        }

        /// <summary>Sets the right padding of the style</summary>
        public static GUIStyle PaddingRight(this GUIStyle style, int right)
        {
            style.padding.right = right;
            return style;
        }

        /// <summary>Sets the top padding of the style</summary>
        public static GUIStyle PaddingTop(this GUIStyle style, int top)
        {
            style.padding.top = top;
            return style;
        }

        /// <summary>Sets the bottom padding of the style</summary>
        public static GUIStyle PaddingBottom(this GUIStyle style, int bottom)
        {
            style.padding.bottom = bottom;
            return style;
        }

        /// <summary>Sets the Y fixedWidth of the style</summary>
        public static GUIStyle Width(this GUIStyle style, float width)
        {
            style.fixedWidth = width;
            return style;
        }

        /// <summary>Sets the fixedHeight of the style</summary>
        public static GUIStyle Height(this GUIStyle style, int height)
        {
            style.fixedHeight = (float) height;
            return style;
        }

        /// <summary>Sets the stretchHeight property of the style</summary>
        public static GUIStyle StretchHeight(this GUIStyle style, bool doStretch = true)
        {
            style.stretchHeight = doStretch;
            return style;
        }

        /// <summary>Sets the stretchWidth property of the style</summary>
        public static GUIStyle StretchWidth(this GUIStyle style, bool doStretch = true)
        {
            style.stretchWidth = doStretch;
            return style;
        }
    }

    public enum Format
    {
        RichText,
        WordWrap,
        NoRichText,
        NoWordWrap,
    }
}