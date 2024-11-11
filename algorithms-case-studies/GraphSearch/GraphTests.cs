using GraphSearch.Problems.Implementations;
using Xunit.Abstractions;

namespace GraphSearch;

public class GraphTests(ITestOutputHelper output)
{
    [Fact(DisplayName = "Should build a read only graph from a encoded problem.")]
    public void ShouldBuildReadOnlyGraph()
    {
        var problem = new SimpleGraphProblem();
        
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);
        
        Assert.Equal(problem.Edges.Keys.Count, graph.NodeValues.Count);
    }

    [Fact(DisplayName = "Should fail to build a read only graph from a encoded problem.")]
    public void ShouldFailBuildReadOnlyGraph()
    {
        var problem = new SimpleBuggedGraphProblem();

        Assert.Throws<InvalidGraphDataException<string>>(() =>
        {
            GraphOrchestrator<string>.CreateReadOnly(problem);
        });
    }
}