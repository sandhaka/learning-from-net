using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourcingSourceGenerator.Option;

internal static class OptionExtensions
{
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
    {
        var result = sequence.FirstOrDefault(predicate);

        if (result is null)
            return new None<T>();

        return new Some<T>(result);
    }
}