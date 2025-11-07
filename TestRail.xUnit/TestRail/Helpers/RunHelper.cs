using Zaghloul.QA.TestRail.xUnit.Helpers;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;

public class RunHelper
{
    public static string GenerateRunName(string projectName, string serviceName)
    {
        var timestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss-fff");

        if (!string.IsNullOrWhiteSpace(serviceName))
            return $"Run-{projectName}-{serviceName}-{timestamp}";

        return $"Run-{projectName}-{timestamp}";
    }

    public static string GenerateRunDescription(string projectName)
    {
        var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
        return $"Automated Run for Project {projectName}, Executed: {date}.\nCreated by xUnit TestRail Plugin v{AssemblyHelper.GetAssemblyVersion()}.";
    }
}