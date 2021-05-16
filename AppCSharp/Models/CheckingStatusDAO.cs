using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class CheckingStatusDAO
    {
        public CheckingStatusDAO()
        {
            PlaceCheckings = new HashSet<PlaceCheckingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PlaceCheckingDAO> PlaceCheckings { get; set; }
    }
}
