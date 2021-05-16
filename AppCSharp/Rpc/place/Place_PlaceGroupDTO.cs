using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.place
{
    public class Place_PlaceGroupDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long? ParentId { get; set; }
        
        public string Name { get; set; }
        
        public string Code { get; set; }
        
        public bool Used { get; set; }
        

        public Place_PlaceGroupDTO() {}
        public Place_PlaceGroupDTO(PlaceGroup PlaceGroup)
        {
            
            this.Id = PlaceGroup.Id;
            
            this.ParentId = PlaceGroup.ParentId;
            
            this.Name = PlaceGroup.Name;
            
            this.Code = PlaceGroup.Code;
            
            this.Used = PlaceGroup.Used;
            
            this.Errors = PlaceGroup.Errors;
        }
    }

    public class Place_PlaceGroupFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ParentId { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Code { get; set; }
        
        public PlaceGroupOrder OrderBy { get; set; }
    }
}