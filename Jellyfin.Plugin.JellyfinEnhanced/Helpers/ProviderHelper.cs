using Jellyfin.Plugin.JellyfinEnhanced.Model.Arr;

namespace Jellyfin.Plugin.JellyfinEnhanced.Helpers
{
    public static class ProviderHelper
    {
        /// <summary>
        /// Returns the ItemId with the highest score based on how many provider IDs match.
        /// Accepts any number of providers.
        /// </summary>
        /// <param name="providers">List of (Provider, Value) tuples.</param>
        /// <param name="itemMap">Dictionary mapping (Provider, Value) to ItemId.</param>
        public static Guid? GetBestItemId(
            IEnumerable<(string Provider, string Value)> providers,
            Dictionary<(string Provider, string Value), Guid> itemMap)
        {
            var scoreMap = new Dictionary<Guid, int>();

            foreach (var (provider, value) in providers)
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    itemMap.TryGetValue((provider, value), out var itemId))
                {
                    scoreMap[itemId] = scoreMap.GetValueOrDefault(itemId) + 1;
                }
            }

            if (scoreMap.Count == 0)
                return null;

            return scoreMap.MaxBy(kv => kv.Value).Key;
        }

        public static List<(string Provider, string Value)> GetProviders(ArrItem e)
        {
            var providers = new List<(string, string)>();

            var tvdb = e.EpisodeTvdbId?.ToString() ?? e.TvdbId?.ToString();
            if (tvdb != null) providers.Add(("Tvdb", tvdb));

            var tmdb = e.TmdbId?.ToString();
            if (tmdb != null) providers.Add(("Tmdb", tmdb));

            var imdb = !string.IsNullOrWhiteSpace(e.EpisodeImdbId) ? e.EpisodeImdbId : e.ImdbId;
            if (!string.IsNullOrWhiteSpace(imdb)) providers.Add(("Imdb", imdb));

            return providers;
        }
    }
}