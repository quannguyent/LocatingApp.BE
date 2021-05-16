using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.place_group
{
    public class PlaceGroup_PlaceGroupDTO : DataDTO
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Used { get; set; }
        public PlaceGroup_PlaceGroupDTO Parent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PlaceGroup_PlaceGroupDTO() {}
        public PlaceGroup_PlaceGroupDTO(PlaceGroup PlaceGroup)
        {
            this.Id = PlaceGroup.Id;
            this.ParentId = PlaceGroup.ParentId;
            this.Name = PlaceGroup.Name;
            this.Code = PlaceGroup.Code;
            this.Used = PlaceGroup.Used;
            this.Parent = PlaceGroup.Parent == null ? null : new PlaceGroup_PlaceGroupDTO(PlaceGroup.Parent);
            this.CreatedAt = PlaceGroup.CreatedAt;
            this.UpdatedAt = PlaceGroup.UpdatedAt;
            this.Errors = PlaceGroup.Errors;
        }
    }

    public class PlaceGroup_PlaceGroupFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Code { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public PlaceGroupOrder OrderBy { get; set; }
    }
}
