using Zaghloul.QA.TestRail.xUnit.Config;
using Zaghloul.QA.TestRail.xUnit.Models;
using Zaghloul.QA.TestRail.xUnit.TestRail.Client;

namespace Zaghloul.QA.TestRail.xUnit
{
    public class Init
    {
        public static async Task<InitResult> Check()
        {
            var result = new InitResult();
            var config = AppConfigHelper.GetAppConfigurations();

            // Check if Publish is Enabled
            // Check if TestRail Email or ApiKey/Password is missing
            // Check if ProjectId isn't provided

            if (!config.PublishResultsEnabled
                || string.IsNullOrWhiteSpace(config.Email)
                || string.IsNullOrWhiteSpace(config.ApiKey)
                || !ulong.TryParse(config.ProjectId, out ulong projectId))
                return result.SetInvalid();
            else
                result.SetValid(config, projectId);


            if (result.IsValid)
            {
                // At this point everything should be ready, final check to see if projectId is valid and credentails works
                try
                {
                    await new TestRailClient().GetProject(result.ProjectId);
                }
                catch (Exception)
                {
                    return result.SetInvalid();
                }
            }

            return result;
        }
    }
}