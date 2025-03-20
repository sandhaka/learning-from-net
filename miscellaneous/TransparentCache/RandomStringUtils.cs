using RandomString4Net;

namespace TransparentCache
{
    public static class RandomStringUtils
    {
        public static IEnumerable<string> RepeatsRandomly()
        {
            var rand = new Random();
            var source = RandomString.GetStrings(Types.ALPHABET_UPPERCASE, 100);

            for(;;) yield return new string(source[rand.Next(0, source.Count - 1)]);
        }
    }
}
