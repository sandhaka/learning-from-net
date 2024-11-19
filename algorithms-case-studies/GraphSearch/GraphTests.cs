using System.Text;
using GraphSearch.Graph.Parameters;
using GraphSearch.Graph.Search;
using Xunit.Abstractions;
using GraphSearch.Problems.Samples;

namespace GraphSearch;

internal class VerifiableTestOutputHelper(ITestOutputHelper output) : ITestOutputHelper
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
    private readonly VerifiableTestOutputHelper _output = new(output);

    [Fact(DisplayName = "Should build a read only graph from a encoded problem.")]
    public void ShouldBuildReadOnlyGraph()
    {
        var problem = new SimpleGraphProblem();

        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);

        Assert.Equal(problem.AdjacencyList.Keys.Count, graph.NodeValues.Count);
    }

    [Fact(DisplayName = "Should fail to build a read only graph from a encoded problem.")]
    public void ShouldFailBuildReadOnlyGraph()
    {
        var problem = new SimpleBuggedGraphProblem();

        Assert.Throws<InvalidGraphDataException<string>>(() => { GraphOrchestrator<string>.CreateReadOnly(problem); });
    }

    [Fact(DisplayName = "Should use an action on graph traversal with DFS.")]
    public void ShouldUseAnActionOnGraphTraversalWithDFS()
    {
        var problem = new SimpleGraphProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);

        graph.OnVisitActionParameter = new OnVisit<string>(value =>
        {
            _output.WriteLine($"Visiting {value}");

            if (value.Equals("M"))
                _output.WriteLine("I'm reached M!");
        });

        // Act
        graph.TraverseDfs("A");

        // Verify
        Assert.Contains("I'm reached M!", _output.GetOutput());
    }

    [Fact(DisplayName = "Should use an action on graph traversal with BFS.")]
    public void ShouldUseAnActionOnGraphTraversalWithBFS()
    {
        var problem = new SimpleGraphProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);

        graph.OnVisitActionParameter = new OnVisit<string>(value =>
        {
            _output.WriteLine($"Visiting {value}");

            if (value.Equals("M"))
                _output.WriteLine("I'm reached M!");
        });

        // Act
        graph.TraverseBfs("A");

        // Verify
        Assert.Contains("I'm reached M!", _output.GetOutput());
    }
    
    [Fact]
    public void ShouldSearchShortestPathWithDijkstra()
    {
        var problem = new PathProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);
        var pathSearch = graph.ToPathSearch();
        var searchStrategy = pathSearch.GraphSearchStrategy;
        
        output.WriteLine($"Using {searchStrategy.Name} search strategy.");
        
        // Act
        var s = pathSearch.Search("A", "Z", out var result);

        var reducedResult = result.Reduce(new SearchResult<string>() { Path = new List<string>(), TotalCost = 0 });
        var resultPath = string.Join(',', reducedResult.Path);
        
        output.WriteLine($"Reached {resultPath} with cost {reducedResult.TotalCost}");
        
        // Verify
        Assert.True(s);
        Assert.Equal("A,B,H,K,Z", resultPath);
        Assert.Equal(12, reducedResult.TotalCost);
    }
    
    [Fact]
    public void ShouldSearchShortestPathWithDijkstraEdgeCaseStartIsTarget()
    {
        var problem = new PathProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);
        var pathSearch = graph.ToPathSearch();
        var searchStrategy = pathSearch.GraphSearchStrategy;
        
        output.WriteLine($"Using {searchStrategy.Name} search strategy.");
        
        // Act
        var s = pathSearch.Search("A", "A", out var result);

        var reducedResult = result.Reduce(new SearchResult<string>() { Path = new List<string>(), TotalCost = 0 });
        var resultPath = string.Join(',', reducedResult.Path);
        
        output.WriteLine($"Reached {resultPath} with cost {reducedResult.TotalCost}");
        
        // Verify
        Assert.True(s);
        Assert.Equal("A", resultPath);
        Assert.Equal(0, reducedResult.TotalCost);
    }
    
    [Fact]
    public void ShouldSearchShortestPathWithDijkstraTargetNotExists()
    {
        var problem = new PathProblem();
        var graph = GraphOrchestrator<string>.CreateReadOnly(problem);
        var pathSearch = graph.ToPathSearch();
        var searchStrategy = pathSearch.GraphSearchStrategy;
        
        output.WriteLine($"Using {searchStrategy.Name} search strategy.");
        
        // Act
        var s = pathSearch.Search("A", "U", out var result);

        var reducedResult = result.Reduce(new SearchResult<string>() { Path = new List<string>(), TotalCost = 0 });
        var resultPath = string.Join(',', reducedResult.Path);
        
        // Verify
        Assert.False(s);
        Assert.True(string.IsNullOrEmpty(resultPath));
    }
}