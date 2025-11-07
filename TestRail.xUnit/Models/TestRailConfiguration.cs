namespace Zaghloul.QA.TestRail.xUnit.Models
{
    public class TestRailConfiguration
    {
        public string Email { get; set; }

        public string ApiKey { get; set; }

        public bool PublishResultsEnabled { get; set; }

        public bool CloseRun { get; set; }

        public bool AddOnlyExecutedCasesToRun { get; set; }

        public string ProjectId { get; set; }

        public string MilestoneName { get; set; }

        public string DefaultSectionName { get; set; }

        public string ServiceName { get; set; }
    }
}