namespace Magis.School.ApiClient.Models
{
    public class VncContainerConnectInfo
    {
        public string NodeIP { get; set; }

        public string VncHost { get; set; }

        public int VncPort { get; set; }

        public int WebsockifyPort { get; set; }

        public string Password { get; set; }

        public string ReadOnlyPassword { get; set; }
    }
}
