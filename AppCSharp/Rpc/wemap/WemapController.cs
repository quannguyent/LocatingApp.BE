using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp.Enums;
using LocatingApp.Repositories;
using LocatingApp.Services.MPlace;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocatingApp.Rpc.wemap
{
    public class WemapRoute : Root
    {
        public const string Default = Rpc + Module + "/wemap";
        public const string GetPlace = Default + "/get-place";
    }
    public partial class WemapController : RpcController
    {
        static string ServerIP = $"https://apis.wemap.asia/";
        static string SearchAPI = "geocode-1/search";
        static string Key = "GqfwrZUEfxbwbnQUhtBMFivEysYIxelQ";
        static RestClient client = new RestClient(ServerIP);

        private ICurrentContext CurrentContext;
        private IPlaceService PlaceService;
        private readonly IUOW UOW;
        public WemapController(
            ICurrentContext CurrentContext,
            IPlaceService PlaceService,
            IUOW UOW)
        {
            this.CurrentContext = CurrentContext;
            this.PlaceService = PlaceService;
            this.UOW = UOW;
        }

        [Route(WemapRoute.GetPlace), HttpPost]
        public async Task<ActionResult<List<Wemap_PlaceDTO>>> GetPlace([FromBody] PlaceSearchParamDTO PlaceSearchParamDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            PlaceSearchParams PlaceSearchParams = await ConvertDTOToEntity(PlaceSearchParamDTO);
            List<Place> Places = new List<Place>();
            foreach (var category in PlaceGroupEnum.PlaceGroupEnumList)
            {
                PlaceSearchParams.Categories = category.Code;
                Places.AddRange(await GetPlaceAsync(PlaceSearchParams));
            }
            await UOW.PlaceRepository.BulkMerge(Places);
            return Places.Select(x => new Wemap_PlaceDTO(x)).ToList();
        }

        private async Task<List<Place>> GetPlaceAsync(PlaceSearchParams PlaceSearchParams)
        {
            List<Place> Places = new List<Place>();

            var Url = ServerIP + SearchAPI;
            var request = new RestRequest(Url, Method.GET);
            request.AddParameter("text", PlaceSearchParams.Text);
            request.AddParameter("location.lat", PlaceSearchParams.LocationLat);
            request.AddParameter("location.lon", PlaceSearchParams.LocationLon);
            request.AddParameter("size", PlaceSearchParams.Size);
            request.AddParameter("key", Key);
            request.AddParameter("bbox.max_lon", PlaceSearchParams.BBoxMaxLon);
            request.AddParameter("bbox.min_lon", PlaceSearchParams.BBoxMinLon);
            request.AddParameter("bbox.max_lat", PlaceSearchParams.BBoxMaxLat);
            request.AddParameter("bbox.min_lat", PlaceSearchParams.BBoxMinLat);
            request.AddParameter("categories", PlaceSearchParams.Categories);

            var result = client.ExecuteAsync(request).Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = (JObject)JsonConvert.DeserializeObject(result.Content);
                var features = content["features"].Children();

                foreach (var feature in features)
                {
                    var Lat = feature["geometry"]["coordinates"][0];
                    var Lon = feature["geometry"]["coordinates"][1];
                    var PlaceId = feature["properties"]["id"].Value<string>();
                    var PlaceName = feature["properties"]["name"].Value<string>();

                    Place Place = new Place
                    {
                        Latitude = decimal.TryParse(Lat.ToString(), out decimal lat) ? lat : 0,
                        Longtitude = decimal.TryParse(Lon.ToString(), out decimal lon) ? lon : 0,
                        PlaceGroupId = PlaceGroupEnum.PlaceGroupEnumList
                            .Where(x => x.Code == PlaceSearchParams.Categories)
                            .Select(x => x.Id).FirstOrDefault(),
                        Name = PlaceName.ToString(),
                        Code = PlaceId.ToString(),
                        Radius = 100
                    };
                    Places.Add(Place);
                }
                    
            }
            return Places;
        }

        private async Task<PlaceSearchParams> ConvertDTOToEntity(PlaceSearchParamDTO DTO)
        {
            var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            var LocationLat = CurrentUser.LocationLogs.LastOrDefault().Latitude;
            var LocationLon = CurrentUser.LocationLogs.LastOrDefault().Longtitude;

            PlaceSearchParams PlaceSearchParams = new PlaceSearchParams
            {
                Text = DTO.Text,
                LocationLat = LocationLat,
                LocationLon = LocationLon,
                BBoxMaxLat = LocationLat + DTO.Rad,
                BBoxMinLat = LocationLat - DTO.Rad,
                BBoxMaxLon = LocationLon + DTO.Rad,
                BBoxMinLon = LocationLon - DTO.Rad,
                Size = DTO.Size,
            };

            return PlaceSearchParams;
        }
    }
}
