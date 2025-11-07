namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models
{
    public class SuiteList
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int Size { get; set; }

        public Links _Links { get; set; }

        public List<Suite> Suites { get; set; }
    }
}