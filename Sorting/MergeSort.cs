namespace Sorting;

public class MergeSort
{
    private readonly Random _random = new();
    private const int ArrayLength = 10000;

    [Fact]
    public void Play()
    {
        // Instantiate a random string of characters
        var arrayOfFunc = Enumerable.Repeat(RandomChar, ArrayLength).ToArray();
        var array = arrayOfFunc.Select(randomChar => randomChar()).ToArray();
        var str = new string(array);

        // Sort the string using the Merge Sort algorithm
        var spanStr = str.AsSpan();
        var sortedSpan = Sort(spanStr);
        
        // Verifying the string is sorted
        Assert.True(IsSorted(sortedSpan));

        return;

        char RandomChar() => (char)_random.Next(32, 127);

        bool IsSorted(ReadOnlySpan<char> s)
        {
            for (var i = 1; i < s.Length; i++)
            {
                if (s[i - 1] > s[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    private static ReadOnlySpan<char> Merge(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
    {
        var result = new char[left.Length + right.Length]; // Heap allocation!

        var i = 0;

        while (left.Length > 0 && right.Length > 0)
        {
            if (left[0] <= right[0])
            {
                result[i++] = left[0];
                left = left[1..];
            }
            else
            {
                result[i++] = right[0];
                right = right[1..];
            }
        }

        if (left.Length > 0)
        {
            left.CopyTo(result.AsSpan(i));
            i += left.Length;
        }

        if (right.Length > 0)
            right.CopyTo(result.AsSpan(i));

        return new ReadOnlySpan<char>(result);
    }

    private static ReadOnlySpan<char> Sort(ReadOnlySpan<char> span)
    {
        if (span.Length<=1) return span;
        var halfIdx = (int)Math.Ceiling(span.Length/2.0);
        var left = span[..halfIdx];
        var right = span[halfIdx..];
        left = Sort(left);
        right = Sort(right);
        return Merge(left, right);
    }
}