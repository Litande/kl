using BenchmarkDotNet.Running;

namespace Plat4Me.DialRuleEngine.BenchmarkTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            //new EngineTests().QueueProcess50kLeadsAnd();
            //new EngineTests().ScoreProcess50kLeadsAnd();
            var summary = BenchmarkRunner.Run<EngineTests>();
        }
    }
}