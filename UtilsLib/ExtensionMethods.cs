using System;
using System.Reflection;
using System.Text;
using System.Threading;

namespace UtilsLib
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// run method in new thread
        /// </summary>
        /// <param name="mi"></param>
        /// <param name="target"></param>
        /// <param name="parameters"></param>
        public static void InvokeOnNewThread(this MethodInfo mi, object target, params object[] parameters)
        {
            ThreadStart threadMain = delegate { mi.Invoke(target, parameters); };
            new Thread(threadMain).Start();
        }

        /// <summary>
        /// Convert string to bytes without encoding
        /// </summary>
        public static byte[] GetBytes(this string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Convert bytes to string without encoding
        /// </summary>
        public static string GetString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// Convert bytes to ascii-string
        /// </summary>
        public static string GetAsciiString(this byte[] bytes, int offset, int length)
        {
            return Encoding.ASCII.GetString(bytes, offset, length);
        }
        
        /// <summary>
        /// Convert ascii-string bytes
        /// </summary>
        public static byte[] GetBytesFromAsciiString(this string ascii)
        {
            return Encoding.ASCII.GetBytes(ascii);
        }
    }
}