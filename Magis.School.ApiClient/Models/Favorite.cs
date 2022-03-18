namespace Magis.School.ApiClient.Models
{
    public class Favorite
    {
        public string Id { get; }

        public string UserName { get; }

        public Enums.FavoriteType Type { get; }

        public string Subject { get; }
    }
}
