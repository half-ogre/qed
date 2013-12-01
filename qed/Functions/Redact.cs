using System.Text.RegularExpressions;

namespace qed
{
    public static partial class Functions
    {
        static readonly Regex _tokenRegex = new Regex(@"(https*\:\/\/)[a-f0-9:]+(@)", RegexOptions.IgnoreCase);
        
        public static string Redact(string text)
        {
            return _tokenRegex.Replace(text, "$1(redacted)$2");
        }
    }
}
