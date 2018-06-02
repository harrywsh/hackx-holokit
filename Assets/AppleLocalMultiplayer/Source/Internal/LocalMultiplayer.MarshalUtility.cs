#if (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX) && (!UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_OVERRIDE)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LostPolygon.Apple.LocalMultiplayer.Internal;

namespace LostPolygon.Apple.LocalMultiplayer {
    public static partial class AppleLocalMultiplayer {
        private static class MarshalUtility {
            /// <summary>
            /// Marshals the native array of pointers to native structs to a managed array.
            /// </summary>
            public static void MarshalNativeArray<T>(IntPtr nativeArrayPtr, int nativeArraySize, T[] results, Func<IntPtr, T> createFunc) where T : class {
                if (nativeArrayPtr == IntPtr.Zero)
                    throw new ArgumentNullException("nativeArrayPtr");

                if (nativeArraySize < 0)
                    throw new ArgumentOutOfRangeException("nativeArraySize");

                if (results.Length < nativeArraySize)
                    throw new ArgumentOutOfRangeException("nativeArraySize", "results.Length < nativeArraySize");

                if (createFunc == null)
                    throw new ArgumentNullException("createFunc");

                IntPtr currentNativeArrayPtr = nativeArrayPtr;
                for (int i = 0; i < nativeArraySize; i++) {
                    IntPtr itemPtr = Marshal.ReadIntPtr(currentNativeArrayPtr);
                    results[i] = createFunc(itemPtr);
                    currentNativeArrayPtr = (IntPtr)(currentNativeArrayPtr.ToInt64() + IntPtr.Size);
                }
            }

            /// <summary>
            /// Marshals the native array of pointers to native structs to a managed array.
            /// </summary>
            public static T[] MarshalNativeArray<T>(IntPtr nativeArrayPtr, int nativeArraySize, Func<IntPtr, T> createFunc) where T : class {
                T[] managedArray = new T[nativeArraySize];
                MarshalNativeArray(nativeArrayPtr, nativeArraySize, managedArray, createFunc);
                return managedArray;
            }

            /// <summary>
            /// Marshals a native linear array of structs to the managed array.
            /// </summary>
            public static T[] MarshalNativeStructArray<T>(IntPtr nativeArrayPtr, int nativeArraySize) where T : struct {
                if (nativeArrayPtr == IntPtr.Zero)
                    throw new ArgumentNullException("nativeArrayPtr");

                if (nativeArraySize < 0)
                    throw new ArgumentOutOfRangeException("nativeArraySize");

                T[] managedArray = new T[nativeArraySize];
                IntPtr currentNativeArrayPtr = nativeArrayPtr;
                int structSize = Marshal.SizeOf(typeof(T));
                for (int i = 0; i < nativeArraySize; i++) {
                    T marshaledStruct = (T) Marshal.PtrToStructure(currentNativeArrayPtr, typeof(T));
                    managedArray[i] = marshaledStruct;
                    currentNativeArrayPtr = (IntPtr) (currentNativeArrayPtr.ToInt64() + structSize);
                }

                return managedArray;
            }

            /// <summary>
            /// Marshals the native representation to a IDictionary&lt;string, string&gt;.
            /// </summary>
            public static Dictionary<string, string> MarshalStringStringDictionary(IntPtr nativePairArrayPtr, int nativePairArraySize) {
                if (nativePairArrayPtr == IntPtr.Zero)
                    throw new ArgumentNullException("nativePairArrayPtr");

                if (nativePairArraySize < 0)
                    throw new ArgumentOutOfRangeException("nativePairArraySize");

                Dictionary<string, string> dictionary = new Dictionary<string, string>(nativePairArraySize);
                NativeMethods.UMC_StringStringKeyValuePair[] pairs = MarshalNativeStructArray<NativeMethods.UMC_StringStringKeyValuePair>(nativePairArrayPtr, nativePairArraySize);
                foreach (NativeMethods.UMC_StringStringKeyValuePair pair in pairs) {
                    dictionary.Add(pair.Key, pair.Value);
                }
                return dictionary;
            }

            /// <summary>
            /// Marshals a IDictionary&lt;string, string&gt; to the native internal representation.
            /// </summary>
            /// <param name="dict">The dictionary.</param>
            /// <returns></returns>
            public static NativeMethods.UMC_StringStringKeyValuePair[] StringStringDictionaryToPairArray(IDictionary<string, string> dict) {
                NativeMethods.UMC_StringStringKeyValuePair[] pairs = new NativeMethods.UMC_StringStringKeyValuePair[dict.Count];

                int counter = 0;
                foreach (KeyValuePair<string, string> keyValuePair in dict) {
                    pairs[counter].Key = keyValuePair.Key;
                    pairs[counter].Value = keyValuePair.Value;
                    counter++;
                }
                return pairs;
            }

            /// <summary>
            /// Marshals data from an unmanaged block of memory to a newly allocated managed object of the specified type.
            /// </summary>
            public static T PtrToStructure<T>(IntPtr ptr) where T : struct {
                return (T) Marshal.PtrToStructure(ptr, typeof(T));
            }
        }
    }
}
#endif
