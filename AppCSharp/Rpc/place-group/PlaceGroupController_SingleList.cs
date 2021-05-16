using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using LocatingApp.Entities;
using LocatingApp.Services.MPlaceGroup;

namespace LocatingApp.Rpc.place_group
{
    public partial class PlaceGroupController : RpcController
    {
        [Route(PlaceGroupRoute.SingleListPlaceGroup), HttpPost]
        public async Task<List<PlaceGroup_PlaceGroupDTO>> SingleListPlaceGroup([FromBody] PlaceGroup_PlaceGroupFilterDTO PlaceGroup_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter();
            PlaceGroupFilter.Skip = 0;
            PlaceGroupFilter.Take = int.MaxValue;
            PlaceGroupFilter.OrderBy = PlaceGroupOrder.Id;
            PlaceGroupFilter.OrderType = OrderType.ASC;
            PlaceGroupFilter.Selects = PlaceGroupSelect.ALL;
            PlaceGroupFilter.Id = PlaceGroup_PlaceGroupFilterDTO.Id;
            PlaceGroupFilter.ParentId = PlaceGroup_PlaceGroupFilterDTO.ParentId;
            PlaceGroupFilter.Name = PlaceGroup_PlaceGroupFilterDTO.Name;
            PlaceGroupFilter.Code = PlaceGroup_PlaceGroupFilterDTO.Code;
            List<PlaceGroup> PlaceGroups = await PlaceGroupService.List(PlaceGroupFilter);
            List<PlaceGroup_PlaceGroupDTO> PlaceGroup_PlaceGroupDTOs = PlaceGroups
                .Select(x => new PlaceGroup_PlaceGroupDTO(x)).ToList();
            return PlaceGroup_PlaceGroupDTOs;
        }
    }
}

