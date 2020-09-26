using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class OpsiProgramDirectories
    {
        public ICollection<string> WindowsPaths { get; set; }

        public ICollection<string> LinuxPaths { get; set; }
    }
}
