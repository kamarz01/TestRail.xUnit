using System.Text.RegularExpressions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Zaghloul.QA.TestRail.xUnit.Helpers
{
    public class TestFrameworkHelper
    {
        public static IEnumerable<string> MatchTestRailInlineData(List<IAttributeInfo> attributes, object[] actualArgs)
        {
            foreach (var attr in attributes)
            {
                var ctorArgs = attr.GetConstructorArguments().ToArray();
                if (ctorArgs.Count() < 2) continue;

                var caseIds = ctorArgs[0] as IEnumerable<string> ?? Array.Empty<string>();
                var expectedArgs = ctorArgs[1] as object[] ?? Array.Empty<object>();

                if (ArgumentsMatch(expectedArgs, actualArgs))
                    return caseIds;
            }

            return null;
        }

        public static string ExtractCaseIdFromInlineData(object[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            return args
                .OfType<string>()
                .FirstOrDefault(s => Regex.IsMatch(s, @"^C\d+", RegexOptions.IgnoreCase));
        }

        public static string GetDisplayName(IXunitTestCase testCase)
        {
            if (string.IsNullOrEmpty(testCase.DisplayName))
                return null;

            var displayName = testCase.DisplayName;

            // Split method name safely before args
            var indexOfParen = displayName.LastIndexOf('(');
            var beforeArgs = indexOfParen >= 0 ? displayName.Substring(0, indexOfParen) : displayName;
            var methodWithArgs = beforeArgs.Split('.').LastOrDefault() + (indexOfParen >= 0 ? displayName.Substring(indexOfParen) : "");

            if (string.IsNullOrWhiteSpace(methodWithArgs))
                return null;

            var nameParts = methodWithArgs.Split('(');
            var methodName = nameParts[0].Replace("_", " ").Trim();

            // Check for CaseId in title:
            methodName = Regex.Replace(methodName, @"C\d+", string.Empty, RegexOptions.IgnoreCase).Trim();

            if (nameParts.Length == 1)
                return methodName;

            var rawArgs = nameParts[1].TrimEnd(')');
            var argsList = rawArgs.Split(',').Select(a => a.Trim()).ToList();

            for (int i = argsList.Count - 1; i >= 0; i--)
            {
                var arg = argsList[i];
                if (string.IsNullOrEmpty(arg))
                    continue;

                var parts = arg.Split(':');
                var key = parts.Length > 1 ? parts[0].Trim().Trim('"') : null;
                var value = parts.Length > 1 ? parts[1].Trim().Trim('"') : null;

                // Remove the CaseId, __pickleIndex, and exampleTags arguments from name
                if (!string.IsNullOrEmpty(value) &&
                     (Regex.IsMatch(value, @"^C\d+", RegexOptions.IgnoreCase) ||
                      key.Contains("pickleIndex", StringComparison.OrdinalIgnoreCase) ||
                      key.Contains("exampleTags", StringComparison.OrdinalIgnoreCase)))
                {
                    argsList.RemoveAt(i);
                }
            }

            return argsList.Count > 0
                ? $"{methodName} ({string.Join(", ", argsList)})"
                : methodName;
        }

        public static string GetTestRailTitleWithLimit(string fullName, int maxLength = 250)
        {
            if (fullName.Length <= maxLength)
                return fullName;

            // Create deterministic hash (e.g. using SHA1, then take first 6 chars)
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var hashBytes = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(fullName));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 6);

            // Reserve space for suffix
            var suffix = $"...-{hash}";
            var cutLength = maxLength - suffix.Length;

            return fullName.Substring(0, cutLength) + suffix;
        }

        private static bool ArgumentsMatch(object[] expected, object[] actual)
        {
            if (expected.Length != actual.Length) return false;

            for (int i = 0; i < expected.Length; i++)
            {
                if (!Equals(expected[i], actual[i]))
                    return false;
            }

            return true;
        }
    }
}