using System;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
     public class VncContainer
    {
      public string Id { get; set; }

        public DateTime CreationTime { get; set; }

        public string AppName { get; set; }

        public string DisplayName { get; set; }

        public string Color { get; set; }

        public IDictionary<string, string> Icons { get; set; }

        public string DockerImage { get; set; }

        public bool IsInternal { get; set; }

        public ICollection<VncContainerAccessor> Accessors { get; set; }

        public Enums.VncContainerStatus Status { get; set; }

        public DateTime LastStatusUpdate { get; set; }

        public string NodeName { get; set; }

        public VncContainerConnectInfo ConnectInfo { get; set; }

        public string DockerContainerName { get; set; }

        public string DockerContainerIp { get; set; }

        public int DockerContainerVncPort { get; set; }

        public string OwnerUserName { get; set; }
    }
}
