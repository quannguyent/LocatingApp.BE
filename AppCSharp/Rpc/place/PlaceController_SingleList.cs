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
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MPlaceGroup;

namespace LocatingApp.Rpc.place
{
    public partial class PlaceController : RpcController
    {
        [Route(PlaceRoute.SingleListPlaceGroup), HttpPost]
        public async Task<List<Place_PlaceGroupDTO>> SingleListPlaceGroup([FromBody] Place_PlaceGroupFilterDTO Place_PlaceGroupFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            PlaceGroupFilter PlaceGroupFilter = new PlaceGroupFilter();
            PlaceGroupFilter.Skip = 0;
            PlaceGroupFilter.Take = int.MaxValue;
            PlaceGroupFilter.OrderBy = PlaceGroupOrder.Id;
            PlaceGroupFilter.OrderType = OrderType.ASC;
            PlaceGroupFilter.Selects = PlaceGroupSelect.ALL;
            PlaceGroupFilter.Id = Place_PlaceGroupFilterDTO.Id;
            PlaceGroupFilter.ParentId = Place_PlaceGroupFilterDTO.ParentId;
            PlaceGroupFilter.Name = Place_PlaceGroupFilterDTO.Name;
            PlaceGroupFilter.Code = Place_PlaceGroupFilterDTO.Code;
            List<PlaceGroup> PlaceGroups = await PlaceGroupService.List(PlaceGroupFilter);
            List<Place_PlaceGroupDTO> Place_PlaceGroupDTOs = PlaceGroups
                .Select(x => new Place_PlaceGroupDTO(x)).ToList();
            return Place_PlaceGroupDTOs;
        }
    }
}

