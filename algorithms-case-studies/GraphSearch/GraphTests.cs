using System.Text;
using Monads.Optional;
using Xunit.Abstractions;

using GraphSearch.Graph;
using GraphSearch.Problems.Implementations;

namespace GraphSearch;

internal class CustomTestOutputHelper(ITestOutputHelper output) : ITestOutputHelper
{
    private readonly StringBuilder _stringBuilder = new();

    public void WriteLine(string message)
    {
        _stringBuilder.AppendLine(message);
        output.WriteLine(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        _stringBuilder.AppendLine(string.Format(format, args));
        output.WriteLine(format, args);
    }

    public string GetOutput()
    {
        return _stringBuilder.ToString().Trim();
    }
}

public class GraphTests(ITestOutputHelper output)
{
    private readonly CustomTestOutputHelper _output = new(output);
    
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
    
    [Fact(DisplayName = "Should use an action on graph traversal.")]
    public void ShouldUseAnActionOnGraphTraversal()
    {
        var problem = new SimpleGraphProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);
        
        graph.ActionParameter = Option<NodeAction<string>>.Some(value =>
        {
            if (value.Equals("M"))
                _output.WriteLine("I'm reached M!");
        });
        
        graph.Dfs("A");
        
        // Verify
        Assert.Contains("I'm reached M!", _output.GetOutput());
    }
}