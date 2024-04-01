using Xunit;

namespace lsp;

public class StackableTests
{
    // Test IStackable<>
    private readonly IStackable<int> _sut = new UniqueStack<int>();
    
    [Fact]
    public void ShouldBeNonEmptyAfterPush1()
    {
        _sut.Push(1);
        Assert.True(_sut.Count > 0);
    }
    
    [Fact]
    public void ShouldBeNonEmptyAfterPush2()
    {
        _sut.Push(1, 2);
        Assert.True(_sut.Count > 0);
    }
    
    [Fact]
    public void ShouldBeNonEmptyAfterPush3()
    {
        _sut.Push(1, 2, 3);
        Assert.True(_sut.Count > 0);
    }

    [Fact]
    public void ShouldPopOnNonEmpty()
    {
        _sut.Push(1, 2, 3);
        var item = _sut.Pop();

        Assert.NotEqual(default(int), item);
    }

    [Fact]
    public void ShouldLastPushedComesOutFirst()
    {
        _sut.Push(1, 2, 3);
        _sut.Push(4);

        var first = _sut.Pop();
        
        Assert.Equal(4, first);
    }
}