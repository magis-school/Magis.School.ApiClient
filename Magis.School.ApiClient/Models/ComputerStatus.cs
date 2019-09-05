using System;

namespace Magis.School.ApiClient.Models
{
    public class ComputerStatus
    {
        public DateTime LastUpdate { get; set; }

        public string LastIpAddress { get; set; }

        public string SessionUserName { get; set; }
    }
}
