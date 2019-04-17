// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public static class SpanSplitTests
    {
        [Fact]
        public static void SplitInvalidOptions()
        {
            ReadOnlySpan<char> value = "a,b";
            const StringSplitOptions optionsTooLow = StringSplitOptions.None - 1;
            const StringSplitOptions optionsTooHigh = StringSplitOptions.RemoveEmptyEntries + 1;

            SpanTestHelpers.AssertThrows<ArgumentException, char>(value, (span) => span.Split(',', optionsTooLow));
            SpanTestHelpers.AssertThrows<ArgumentException, char>(value, (span) => span.Split(',', optionsTooHigh));
        }

        [Fact]
        public static void SplitEmptyValueWithRemoveEmptyEntriesOptionEmptyResult()
        {
            ReadOnlySpan<char> value = string.Empty;
            const StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;

            var enumerator = value.Split(',', options);
            Assert.False(enumerator.MoveNext());
            Assert.Equal(0, enumerator.Current.Length);

            enumerator = value.Split(',', options);
            Assert.Equal(0, enumerator.Current.Length);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public static void SplitNoMatchSingleResult()
        {
            ReadOnlySpan<char> value = "a b";
            const StringSplitOptions options = StringSplitOptions.None;

            string expected = value.ToString();
            var enumerator = value.Split(',', options);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(expected, new string(enumerator.Current));
        }

        [Theory]
        [InlineData("", ',', StringSplitOptions.None, new[] {""})]
        [InlineData("", ',', StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', StringSplitOptions.None, new[] {"", ""})]
        [InlineData(",", ',', StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', StringSplitOptions.None, new[] {"", "", ""})]
        [InlineData(",,", ',', StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("ab", ',', StringSplitOptions.None, new[] {"ab"})]
        [InlineData("ab", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"ab"})]
        [InlineData("a,b", ',', StringSplitOptions.None, new[] {"a", "b"})]
        [InlineData("a,b", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b"})]
        [InlineData("a,", ',', StringSplitOptions.None, new[] {"a", ""})]
        [InlineData("a,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a"})]
        [InlineData(",b", ',', StringSplitOptions.None, new[] {"", "b"})]
        [InlineData(",b", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"b"})]
        [InlineData(",a,b", ',', StringSplitOptions.None, new[] {"", "a", "b"})]
        [InlineData(",a,b", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b"})]
        [InlineData("a,b,", ',', StringSplitOptions.None, new[] {"a", "b", ""})]
        [InlineData("a,b,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b"})]
        [InlineData("a,b,c", ',', StringSplitOptions.None, new[] {"a", "b", "c"})]
        [InlineData("a,b,c", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b", "c"})]
        [InlineData("a,,c", ',', StringSplitOptions.None, new[] {"a", "", "c"})]
        [InlineData("a,,c", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "c"})]
        [InlineData(",a,b,c", ',', StringSplitOptions.None, new[] {"", "a", "b", "c"})]
        [InlineData(",a,b,c", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b", "c"})]
        [InlineData("a,b,c,", ',', StringSplitOptions.None, new[] {"a", "b", "c", ""})]
        [InlineData("a,b,c,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b", "c"})]
        [InlineData(",a,b,c,", ',', StringSplitOptions.None, new[] {"", "a", "b", "c", ""})]
        [InlineData(",a,b,c,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"a", "b", "c"})]
        [InlineData("first,second", ',', StringSplitOptions.None, new[] {"first", "second"})]
        [InlineData("first,second", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second"})]
        [InlineData("first,", ',', StringSplitOptions.None, new[] {"first", ""})]
        [InlineData("first,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first"})]
        [InlineData(",second", ',', StringSplitOptions.None, new[] {"", "second"})]
        [InlineData(",second", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"second"})]
        [InlineData(",first,second", ',', StringSplitOptions.None, new[] {"", "first", "second"})]
        [InlineData(",first,second", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second"})]
        [InlineData("first,second,", ',', StringSplitOptions.None, new[] {"first", "second", ""})]
        [InlineData("first,second,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second"})]
        [InlineData("first,second,third", ',', StringSplitOptions.None, new[] {"first", "second", "third"})]
        [InlineData("first,second,third", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second", "third"})]
        [InlineData("first,,third", ',', StringSplitOptions.None, new[] {"first", "", "third"})]
        [InlineData("first,,third", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "third"})]
        [InlineData(",first,second,third", ',', StringSplitOptions.None, new[] {"", "first", "second", "third"})]
        [InlineData(",first,second,third", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second", "third"})]
        [InlineData("first,second,third,", ',', StringSplitOptions.None, new[] {"first", "second", "third", ""})]
        [InlineData("first,second,third,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second", "third"})]
        [InlineData(",first,second,third,", ',', StringSplitOptions.None, new[] {"", "first", "second", "third", ""})]
        [InlineData(",first,second,third,", ',', StringSplitOptions.RemoveEmptyEntries, new[] {"first", "second", "third"})]
        [InlineData("Foo Bar Baz", ' ', StringSplitOptions.None, new[] {"Foo", "Bar", "Baz"})]
        [InlineData("Foo Bar Baz", ' ', StringSplitOptions.RemoveEmptyEntries, new[] {"Foo", "Bar", "Baz"})]
        [InlineData("Foo Bar Baz ", ' ', StringSplitOptions.None, new[] {"Foo", "Bar", "Baz", ""})]
        [InlineData("Foo Bar Baz ", ' ', StringSplitOptions.RemoveEmptyEntries, new[] {"Foo", "Bar", "Baz"})]
        [InlineData(" Foo Bar Baz ", ' ', StringSplitOptions.None, new[] {"", "Foo", "Bar", "Baz", ""})]
        [InlineData(" Foo Bar Baz ", ' ', StringSplitOptions.RemoveEmptyEntries, new[] {"Foo", "Bar", "Baz"})]
        public static void SplitCharSeparator(string valueParam, char separator, StringSplitOptions options,
            string[] expectedParam)
        {
            ReadOnlySpan<char> value = valueParam;
            char[][] expected = expectedParam.Select(x => x.ToCharArray()).ToArray();
            SplitHelpers.AssertEqual(value.Split(separator, options), expected);
        }

        private static class SplitHelpers
        {
            public static void AssertEqual<T>(SpanSplitEnumerator<T> source, T[][] items) where T : IEquatable<T>
            {
                foreach (var item in items)
                {
                    Assert.True(source.MoveNext());
                    for (int index = 0; index < item.Length; index++)
                    {
                        Assert.Equal(item[index], source.Current[index]);
                    }
                }

                Assert.False(source.MoveNext());
            }
        }
    }
}
