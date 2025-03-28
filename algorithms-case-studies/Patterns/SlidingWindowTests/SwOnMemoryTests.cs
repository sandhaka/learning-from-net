using SlidingWindowSample.Data;
using SlidingWindowSample.SW;
using SlidingWindowTests.Accumulators;

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

            // Go forward as possible by 1 
            for (var i = 1; i <= dimension - windowSize; i++)
            {
                sw.Advance(1);

                Assert.Equal(9 + i, sw.Head);
                Assert.Equal(0 + i, sw.Tail);
                Assert.Equal(windowSize, sw.Length);
            }

            Assert.Equal(testSeq[90], sw.Tail);
            Assert.Equal(testSeq[99], sw.Head);

            // Go backward until window will placed at the beginning
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

            // Should throw if window is forced to go out of range
            Assert.Throws<InvalidOperationException>(() =>
            {
                sw.FallBack(1);
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                sw.Advance(101);
            });
        }

        [Fact]
        public void ShouldSlideOnListOfWorkTasksWithOneItemWindow()
        {
            var tasksEnumerable = Generator.SampleWorkTasks(1000);
            var tasksList = tasksEnumerable.ToArray();

            // Implicit create a window by one element
            var sw = SlidingWindowFactory.Create(tasksList);

            sw.AddAccumulator(new WorkTasksAccumulator());

            Assert.Equal(tasksList[0], sw.Head);
            Assert.Equal(tasksList[0], sw.Tail);

            sw.Advance(100);

            // Head will point to the 101th item
            Assert.Equal(tasksList[100], sw.Head);
            Assert.Equal(tasksList[100], sw.Tail);

            // Accumulator should work on 1 item window
            var totalWorkData = tasksList.Take(100).Aggregate((wia, wib) => 
                new WorkTask
                {
                    Effort = wia.Effort + wib.Effort, 
                    Deviation = wia.Deviation > wib.Deviation ? wia.Deviation : wib.Deviation, 
                    Value = wia.Value + wib.Value
                });

            // TODO: test accumulators here
        }
    }
}