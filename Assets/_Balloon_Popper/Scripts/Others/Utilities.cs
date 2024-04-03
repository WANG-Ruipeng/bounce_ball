using System;
using UnityEngine;

namespace ClawbearGames
{
    public class Utilities
    {

        /// <summary>
        /// Is finished tutorial.
        /// </summary>
        /// <returns></returns>
        public static bool IsFinishedTutorial()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_TUTORIAL, 0) == 1;
        }


        /// <summary>
        /// Convert color to hex
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        /// <summary>
        /// Convert hex to color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
