using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class PageDAO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
