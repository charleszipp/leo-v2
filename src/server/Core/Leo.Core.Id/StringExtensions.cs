using System;
using System.Text;

namespace Leo.Core.Id
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string id)
        {
            Guid parsedGuid = Guid.Empty;
            if (Guid.TryParse(id, out parsedGuid))
                return parsedGuid;

            byte[] stringbytes = Encoding.UTF8.GetBytes(id);
            byte[] hashedBytes = new System.Security.Cryptography
                .SHA1CryptoServiceProvider()
                .ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }
    }
}