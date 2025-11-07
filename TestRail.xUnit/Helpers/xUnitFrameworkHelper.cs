using System.Collections.Concurrent;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using Zaghloul.QA.TestRail.xUnit.Attributes;
using Zaghloul.QA.TestRail.xUnit.Models;

namespace Zaghloul.QA.TestRail.xUnit.Helpers
{
    public class xUnitFrameworkHelper
    {
        public ConcurrentDictionary<string, TestInfo> ExtractMetadata(IEnumerable<IXunitTestCase> testCases)
        {
            var result = new ConcurrentDictionary<string, TestInfo>();

            foreach (var testCase in testCases)
            {
                IAttributeInfo testRailMetadataAttr;
                var testClass = testCase.TestMethod.TestClass.Class;
                var testMethod = testCase.TestMethod.Method;
                var testArgs = testCase.TestMethodArguments;
                var testId = testCase.UniqueID;
                var finalCaseIds = new List<string>();

                var classMetadataAttr = testClass
                    .GetCustomAttributes(typeof(TestRailMetadataAttribute))
                    .FirstOrDefault();

                if (classMetadataAttr != null)
                    // Use only class-level metadata
                    testRailMetadataAttr = classMetadataAttr;
                else
                {
                    // Fallback to method-level if class-level is not found
                    testRailMetadataAttr = testMethod
                        .GetCustomAttributes(typeof(TestRailMetadataAttribute))
                        .FirstOrDefault();
                }

                // If neither class nor method has the attribute, skip this test
                if (testRailMetadataAttr == null)
                    continue;

                var suiteName = testRailMetadataAttr
                    .GetNamedArgument<string>(nameof(TestRailMetadataAttribute.SuiteName));

                var sectionName = testRailMetadataAttr
                    .GetNamedArgument<string>(nameof(TestRailMetadataAttribute.SectionName));

                // Check if using a class level metadata
                var usingClassLevelMetadata = classMetadataAttr != null;
                if (!usingClassLevelMetadata)
                {
                    // 1. Match TestRailInlineData
                    var testRailInlineAttrs = testMethod
                        .GetCustomAttributes(typeof(TestRailInlineDataAttribute)).ToList();

                    if (testRailInlineAttrs.Any())
                    {
                        var matched = TestFrameworkHelper.MatchTestRailInlineData(testRailInlineAttrs, testArgs);
                        if (matched != null)
                            finalCaseIds.AddRange(matched);
                    }

                    // 2. Match regular InlineData, last param as caseId
                    else if (testMethod.GetCustomAttributes(typeof(InlineDataAttribute)).Any())
                    {
                        var caseId = TestFrameworkHelper.ExtractCaseIdFromInlineData(testArgs);
                        if (!string.IsNullOrWhiteSpace(caseId))
                            finalCaseIds.Add(caseId);
                    }

                    // 3. Fallback to TestRail metadata attribute case IDs
                    else
                    {
                        var attrCaseIds = testRailMetadataAttr
                            .GetNamedArgument<string[]>(nameof(TestRailMetadataAttribute.CaseIds));

                        if (attrCaseIds != null)
                            finalCaseIds.AddRange(attrCaseIds);
                    }
                }

                var displayName = TestFrameworkHelper.GetDisplayName(testCase);

                result.TryAdd(testId, new TestInfo
                {
                    UniqueId = testId,
                    DisplayName = TestFrameworkHelper.GetTestRailTitleWithLimit(displayName),
                    SectionName = sectionName,
                    SuiteName = suiteName,
                    CaseIds = finalCaseIds.Distinct().ToList()
                });
            }

            return result;
        }
    }
}