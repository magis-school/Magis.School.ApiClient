using System.Collections;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class OpsiVersion
    {
        public string Version { get; set; }

        public bool External { get; set; }

        public string ProductId { get; set; }

        public ICollection<string> Replaces { get; set; }

        public OpsiProgramDirectories ProgramDirectories { get; set; }

        public string SourceCode { get; set; }

        public string Author { get; set; }

        public string DownloadPath { get; set; }

        public string DownloadChecksum { get; set; }
    }
}
