﻿using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Vayosoft.Utilities
{
    public static class GuidUtils
    {
        private const char EqualsChar = '=';
        private const char Slash = '/';
        private const char Underscore = '_';
        private const char Hyphen = '-';
        private const byte SlashByte = (byte)'/';
        private const char Plus = '+';
        private const byte PlusByte = (byte)'+';

        public static string GetStringFromGuid(Guid id)
        {
            Span<byte> idBytes = stackalloc byte[16];
            Span<byte> base64Bytes = stackalloc byte[24];

            MemoryMarshal.TryWrite(idBytes, in id);
            Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);

            Span<char> result = stackalloc char[22];
            for (var i = 0; i < 22; i++)
            {
                result[i] = base64Bytes[i] switch
                {
                    SlashByte => Hyphen,
                    PlusByte => Underscore,
                    _ => (char) base64Bytes[i]
                };
            }

            return new string(result);
        }

        public static Guid GetGuidFromString(ReadOnlySpan<char> id)
        {
            Span<char> base64Chars = stackalloc char[24];
            for (var i = 0; i < 22; i++)
            {
                base64Chars[i] = id[i] switch
                {
                    Hyphen => Slash,
                    Underscore => Plus,
                    _ => id[i]
                };
            }
            base64Chars[22] = EqualsChar;
            base64Chars[23] = EqualsChar;

            Span<byte> idBytes = stackalloc byte[16];
            Convert.TryFromBase64Chars(base64Chars, idBytes, out _);
            return new Guid(idBytes);
        }

        public static string CreateUid(bool upperCase = false)
        {
            var guid = Guid.NewGuid().ToString("N");
            return upperCase ? guid.ToUpper() : guid;
        }
    }
}
