using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class PlaceGroupDAO
    {
        public PlaceGroupDAO()
        {
            InverseParent = new HashSet<PlaceGroupDAO>();
        }

        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long Level { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual PlaceGroupDAO Parent { get; set; }
        public virtual ICollection<PlaceGroupDAO> InverseParent { get; set; }
    }
}
