using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class JobParameterDAO
    {
        public long JobId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual JobDAO Job { get; set; }
    }
}
