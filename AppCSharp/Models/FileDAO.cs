using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class FileDAO
    {
        public long Id { get; set; }
        public string GridId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public string Content { get; set; }
    }
}
