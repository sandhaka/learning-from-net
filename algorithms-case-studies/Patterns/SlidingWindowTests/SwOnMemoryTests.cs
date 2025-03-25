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

        public void ShouldMoveWindowByForwardAndBackward()
        {
            const int dimension = 100;
            var testSeq = Enumerable.Range(0, dimension).ToArray();
            var sw = SlidingWindowFactory.Create(testSeq, 0, 10);

            for (var i = 0; i < dimension - 10; i++)
            {
                sw.Advance(1);

                Assert.Equal(9 + i, sw.Head);
                Assert.Equal(0 + i, sw.Tail);
            }

            Assert.Equal(dimension - 1, sw.Head);

            for (var i = dimension - 1; i >= 0; i--)
            {
                sw.FallBack(1);

                Assert.Equal(9 + i, sw.Head);
                Assert.Equal(0 + i, sw.Tail);
            }
        }
    }
}