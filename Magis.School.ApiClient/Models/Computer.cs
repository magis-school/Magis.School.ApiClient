using System;

namespace Magis.School.ApiClient.Models
{
    public class Computer
    {
        public string ComputerName { get; set; }

        public string Hostname { get; set; }

        public string DisplayName { get; set; }

        public string RoomName { get; set; }

        public Enums.ComputerType Type { get; set; }

        public DateTime CreationTime { get; set; }

        public string DN { get; set; }

        public string SID { get; set; }

        public uint PrimaryGroupID { get; set; }
    }
}
