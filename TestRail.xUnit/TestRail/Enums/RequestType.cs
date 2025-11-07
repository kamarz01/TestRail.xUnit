using System.ComponentModel;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Enums;

public enum RequestType
{
    [Description("get")]
    Get,

    [Description("add")]
    Add,

    [Description("update")]
    Update,

    [Description("delete")]
    Delete,

    [Description("close")]
    Close
}