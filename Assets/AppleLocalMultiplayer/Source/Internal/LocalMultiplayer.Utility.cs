#if UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        public static class Utility {
            private static readonly Regex kServiceTypeInvalidSymbolsRegex = new Regex(@"[^a-z\-\d]");

            /// <summary>
            /// Transforms a string into a valid service type name. It can be up to 15 characters long,
            /// valid characters are ASCII lowercase letters, numbers, and the hyphen.
            /// </summary>
            public static string StringToValidServiceTypeName(string str) {
                if (str == null)
                    return null;

                str = str.ToLowerInvariant();
                str = kServiceTypeInvalidSymbolsRegex.Replace(str, "");
                str = TruncateStringToEncodingBytes(str, 15, Encoding.ASCII);

                return str;
            }

            /// <summary>
            /// Truncates the strings to a number of bytes in a given encoding.
            /// </summary>
            /// <param name="str">String to truncate</param>
            /// <param name="maxByteCount">Maximum number of bytes the string can take in <paramref name="encoding"/></param>
            /// <param name="encoding">Target encoding. UTF8 is used if null</param>
            public static string TruncateStringToEncodingBytes(string str, int maxByteCount, Encoding encoding = null) {
                if (str == null)
                    return null;

                if (encoding == null) {
                    encoding = Encoding.UTF8;
                }

                int byteCount = encoding.GetByteCount(str);
                if (byteCount <= maxByteCount)
                    return str;

                // Get first chars, no more than maxByteCount
                char[] chars = str.ToCharArray(0, str.Length > maxByteCount ? maxByteCount : str.Length);
                int targetCharIndex = 0;
                for (int i = chars.Length; i >= 1; i--) {
                    byteCount = encoding.GetByteCount(chars, 0, i);
                    if (byteCount <= maxByteCount) {
                        targetCharIndex = i;
                        break;
                    }
                }

                str = new String(chars, 0, targetCharIndex);
                return str;
            }
        }
    }
}

#endif
