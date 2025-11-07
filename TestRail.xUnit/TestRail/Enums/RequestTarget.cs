using System.ComponentModel;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Enums;

public enum RequestTarget
{
    [Description("run")]
    Run,

    [Description("results_for_cases")]
    ResultForCases,

    [Description("project")]
    Project,

    [Description("projects")]
    Projects,

    [Description("suites")]
    Suites
}