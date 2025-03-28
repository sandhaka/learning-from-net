using SlidingWindowSample.SW;

namespace SlidingWindowTests
{
    public class SwOnMemoryTests
    {
        [Fact]
        public void ShouldCreateASlidingWindowOnSequenceOfInteger()
        {
            var testSeq = Enumerable.Range(0, 11).ToArray();

            // Act
            var sw = SlidingWindowFactory.Create(testSeq, 0, 10);

            Assert.NotNull(sw);
            Assert.Equal(0, sw.Tail);
            Assert.Equal(9, sw.Head);
            Assert.Equal(10, sw.Length);
        }

        [Fact]
        public void ShouldMoveWindowForwardAndBackward()
        {
            const int dimension = 100;
            const int windowSize = 10;

            var testSeq = Enumerable.Range(0, dimension).ToArray();
            var sw = SlidingWindowFactory.Create(testSeq, 0, windowSize);

            for (var i = 1; i <= dimension - windowSize; i++)
            {
                sw.Advance(1);

                Assert.Equal(9 + i, sw.Head);
                Assert.Equal(0 + i, sw.Tail);
                Assert.Equal(windowSize, sw.Length);
            }

            Assert.Equal(testSeq[90], sw.Tail);
            Assert.Equal(testSeq[99], sw.Head);

            for (var i = dimension - 1; i >= windowSize; i--)
            {
                sw.FallBack(1);

                Assert.Equal(i - 1, sw.Head);
                Assert.Equal(i - windowSize, sw.Tail);
                Assert.Equal(windowSize, sw.Length);
            }

            Assert.Equal(testSeq[0], sw.Tail);
            Assert.Equal(testSeq[9], sw.Head);
            Assert.Equal(windowSize, sw.Length);
        }
    }
}