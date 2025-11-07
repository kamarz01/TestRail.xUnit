namespace Zaghloul.QA.TestRail.xUnit.Models
{
    public class InitResult
    {
        public bool IsValid { get; set; }

        public TestRailConfiguration Configuration { get; set; }

        public ulong ProjectId { get; set; }

        public InitResult SetInvalid()
        {
            IsValid = false;
            Configuration = null;
            ProjectId = 0;

            return this;
        }

        public void SetValid(TestRailConfiguration config, ulong projectId)
        {
            IsValid = true;
            Configuration = config;
            ProjectId = projectId;
        }
    }
}