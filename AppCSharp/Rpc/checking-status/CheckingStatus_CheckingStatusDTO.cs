using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.checking_status
{
    public class CheckingStatus_CheckingStatusDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CheckingStatus_CheckingStatusDTO() {}
        public CheckingStatus_CheckingStatusDTO(CheckingStatus CheckingStatus)
        {
            this.Id = CheckingStatus.Id;
            this.Code = CheckingStatus.Code;
            this.Name = CheckingStatus.Name;
            this.Errors = CheckingStatus.Errors;
        }
    }

    public class CheckingStatus_CheckingStatusFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public CheckingStatusOrder OrderBy { get; set; }
    }
}
