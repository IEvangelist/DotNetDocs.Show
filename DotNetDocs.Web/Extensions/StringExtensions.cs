using Microsoft.AspNetCore.DataProtection;

namespace DotNetDocs.Web.Extensions
{
    public static class StringExtensions
    {
        const string TheDotNetDocs = nameof(TheDotNetDocs);

        public static string Encrypt(this string value, IDataProtectionProvider protectionProvider) =>
            protectionProvider.CreateProtector(TheDotNetDocs).Protect(value);

        public static string Decrypt(this string value, IDataProtectionProvider protectionProvider) =>
            protectionProvider.CreateProtector(TheDotNetDocs).Unprotect(value);
    }
}
