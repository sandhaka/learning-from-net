using SlidingWindowSample.SW;
using SlidingWindowTests.Accumulators;
using SlidingWindowTests.Data;

namespace SlidingWindowTests
{
    public class SwOnMemoryTests
    {
        [Fact]
        public void ShouldCreateASlidingWindowOnSequenceOfInteger()
        {
            var testSeq = Enumerable.Range(0, 11).ToArray();

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
            var accumulator = new WorkTasksAccumulator();

            // Implicit create a window by one element
            var sw = SlidingWindowFactory.Create(tasksList);

            sw.AddAccumulator(accumulator);

            Assert.Equal(tasksList[0], sw.Head);
            Assert.Equal(tasksList[0], sw.Tail);

            sw.Advance(100);

            // Head will point to the 101th item
            Assert.Equal(tasksList[100], sw.Head);
            Assert.Equal(tasksList[100], sw.Tail);

            // Accumulator should work on 1 item window
            var workData = tasksList[100];
            var accWValue = accumulator.Value;
            Assert.Equal(workData.Effort, accWValue.Effort);
            Assert.Equal(workData.Deviation, accWValue.Deviation);
            Assert.Equal(workData.Value, accWValue.Value);
        }

        [Fact]
        public void ShouldSlideOnListOfWorkTasks()
        {
            var tasksList = Generator.SampleWorkTasks(1000).ToArray();
            var accumulator = new WorkTasksAccumulator();
            var sw = SlidingWindowFactory.Create(tasksList, 0, 400);
            sw.AddAccumulator(accumulator);

            Assert.Equal(tasksList[399], sw.Head);
            Assert.Equal(tasksList[0], sw.Tail);

            sw.Advance(100);

            Assert.Equal(tasksList[499], sw.Head); // 500th item
            Assert.Equal(tasksList[100], sw.Tail); // 101th item

            var totalWorkData = tasksList.Skip(100).Take(400).Aggregate((wia, wib) => 
                new WorkTask
                {
                    Effort = wia.Effort + wib.Effort, 
                    Deviation = wia.Deviation > wib.Deviation ? wia.Deviation : wib.Deviation, 
                    Value = wia.Value + wib.Value
                });

            var accWValue = accumulator.Value;
            Assert.Equal(totalWorkData.Effort, accWValue.Effort);
            Assert.Equal(totalWorkData.Deviation, accWValue.Deviation);
            Assert.Equal(totalWorkData.Value, accWValue.Value);
        }
    }
}