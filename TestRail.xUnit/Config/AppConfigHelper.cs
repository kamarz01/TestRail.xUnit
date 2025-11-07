using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Zaghloul.QA.TestRail.xUnit.Models;

namespace Zaghloul.QA.TestRail.xUnit.Config
{
    public class AppConfigHelper
    {
        private const string ConfigFileName = "testrailSettings.json";

        public static TestRailConfiguration GetAppConfigurations()
        {
            var configs = GetConfigurations()?.Get<TestRailConfiguration>();

            return configs ?? new TestRailConfiguration
            {
                PublishResultsEnabled = false
            };
        }

        private static IConfigurationRoot GetConfigurations()
        {
            try
            {
                var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile(ConfigFileName, optional: true, reloadOnChange: false);

                var configurations = configBuilder.Build();

                if (!File.Exists(ConfigFileName))
                {
                    Debug.WriteLine($"[xUnit-TestRail-Plugin] Config file '{ConfigFileName}' not found. Using default settings.");
                }

                return configurations;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Failed to load configuration. Reason: {ex.Message}");
                return null;
            }
        }
    }
}