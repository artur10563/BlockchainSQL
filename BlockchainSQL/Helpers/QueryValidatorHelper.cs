using System.Text.RegularExpressions;

namespace BlockchainSQL.Helpers;

public class QueryValidatorHelper
{
    private static readonly Regex SuspiciousRegex = new Regex(
        @"(--|/\*|\*/|;|'|""|\\|\b(or|and|not|like|between|select|insert|update|delete|drop|truncate|alter|exec(ute)?|union|xp_|sp_|shutdown|waitfor\s+delay)\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public virtual bool IsSuspiciousParam(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return SuspiciousRegex.IsMatch(value);
    }
}