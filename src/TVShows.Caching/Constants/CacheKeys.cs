namespace TVShows.Caching
{
    // TODO Needs to be adapted
    public static class CacheKeys
    {
        public const string CacheKeyForShow = "showIds";

        public static string GetCacheKeyForShow(int id) => $"show-{id}";
    }
}
