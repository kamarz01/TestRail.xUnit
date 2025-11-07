using Reqnroll;

namespace Zaghloul.QA.TestRail.xUnit.Bdd.Helpers
{
    public class ScenarioTagHelper
    {
        private static readonly string[] SuiteTagPrefixes = new[] { "SuiteName:", "Suite:" };

        private static readonly string[] SectionTagPrefixes = new[] { "SectionName:", "Section:" };

        public static (string SuiteName, string SectionName) GetSuiteAndSectionTags(FeatureContext scenarioContext)
        {
            if (scenarioContext == null)
                return (null, null);

            var tags = scenarioContext.FeatureInfo.Tags ?? Array.Empty<string>();

            string suiteName = null;
            string sectionName = null;

            foreach (var tag in tags)
            {
                if (StartsWithAny(tag, SuiteTagPrefixes))
                {
                    suiteName = GetTagValue(tag);
                }
                else if (StartsWithAny(tag, SectionTagPrefixes))
                {
                    sectionName = GetTagValue(tag);
                }
            }

            return (suiteName, sectionName);
        }

        private static bool StartsWithAny(string value, params string[] prefixes) =>
            prefixes.Any(p => value.StartsWith(p, StringComparison.OrdinalIgnoreCase));

        private static string GetTagValue(string tag) =>
            tag.Split(':', 2).ElementAtOrDefault(1)?.Trim();
    }
}