namespace Magis.School.ApiClient.Models
{
    public class Favorite
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public Enums.FavoriteType Type { get; set; }

        public string Subject { get; set; }
    }
}
