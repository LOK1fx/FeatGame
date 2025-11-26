
namespace LOK1game.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// https://ru.wikipedia.org/wiki/FNV
        /// Just a low collision hash function
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ComputeFNV1aHash(this string s)
        {
            uint hash = 2166136261;

            foreach(char c in s)
            {
                hash = (hash ^ c) * 16777619;
            }

            return unchecked((int)hash);
        }
    }
}
