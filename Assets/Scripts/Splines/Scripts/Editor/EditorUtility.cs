using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomUI
{
    public static class EditorUtility
    {
        public static string LimitDecimals (float value, int decimals)
        {
            decimals = Mathf.Abs(decimals);
            if (decimals == 0)
                return ((int)value).ToString();
            else
            {
                int mVal = (int)(value * (10 * decimals));
                return (((float)mVal) / (10 * decimals)).ToString();
            }
        }
    }

    public static class UIStyles
    {
        public static GUIStyle Bold
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }

        public static GUIStyle Itallic
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.Italic;
                return style;
            }
        }

        public static GUIStyle BoldItallic
        {
            get
            {
                GUIStyle style = new GUIStyle();
                style.fontStyle = FontStyle.BoldAndItalic;
                return style;
            }
        }

        public static GUIStyle GetStyle (UIStyle style)
        {
            switch (style)
            {
                case UIStyle.Bold:
                    return Bold;
                case UIStyle.BoldItallic:
                    return BoldItallic;
                case UIStyle.Itallic:
                    return Itallic;
            }
            return null;
        } 

        public enum UIStyle
        {
            Bold, Itallic, BoldItallic
        }
    }
}
