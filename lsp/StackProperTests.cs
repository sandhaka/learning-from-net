using Xunit;

namespace lsp;

public class StackProperTests
{
    // Test IStackProper<>
    private readonly IStackProper<int> _sut = new Stack<int>();
    
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

    // These regards only IStackProper<>
    
    [Fact]
    public void ShouldStackItemsComeInLifoOrder()
    {
        _sut.Push(1, 2, 1, 2, 1, 2, 3, 3, 1);
        
        Assert.Equal(1, _sut.Pop());
        Assert.Equal(3, _sut.Pop());
        Assert.Equal(3, _sut.Pop());
        Assert.Equal(2, _sut.Pop());
        Assert.Equal(1, _sut.Pop());
        Assert.Equal(2, _sut.Pop());
        Assert.Equal(1, _sut.Pop());
        Assert.Equal(2, _sut.Pop());
        Assert.Equal(1, _sut.Pop());
    }

    [Fact]
    public void ShouldIncrementStackSize()
    {
        _sut.Push(1, 1, 2);
        
        _sut.Push(1);
        
        Assert.Equal(4, _sut.Count);
    }
    
    [Fact]
    public void ShouldDecrementStackSize()
    {
        _sut.Push(1, 1, 2, 2);
        
        _sut.Pop();
        
        Assert.Equal(3, _sut.Count);
    }
}