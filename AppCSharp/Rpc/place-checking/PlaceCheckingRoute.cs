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
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.place_checking
{
    public class PlaceCheckingRoute : Root
    {
        public const string Parent = Module + "/place-checking";
        public const string Master = Module + "/place-checking/place-checking-master";
        public const string Detail = Module + "/place-checking/place-checking-detail";
        public const string Preview = Module + "/place-checking/place-checking-preview";
        private const string Default = Rpc + Module + "/place-checking";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListPlace = Default + "/filter-list-place";
        public const string FilterListCheckingStatus = Default + "/filter-list-checking-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListPlace = Default + "/single-list-place";
        public const string SingleListCheckingStatus = Default + "/single-list-checking-status";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(PlaceCheckingFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PlaceCheckingFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(PlaceCheckingFilter.PlaceId), FieldTypeEnum.ID.Id },
            { nameof(PlaceCheckingFilter.PlaceCheckingStatusId), FieldTypeEnum.ID.Id },
            { nameof(PlaceCheckingFilter.CheckInAt), FieldTypeEnum.DATE.Id },
            { nameof(PlaceCheckingFilter.CheckOutAt), FieldTypeEnum.DATE.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListPlace,FilterListCheckingStatus,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListPlace, SingleListCheckingStatus, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
