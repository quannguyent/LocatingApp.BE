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
using LocatingApp.Services.MTracking;
using LocatingApp.Services.MPlace;
using LocatingApp.Services.MPlaceChecking;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.tracking
{
    public class TrackingRoute : Root
    {
        public const string Parent = Module + "/tracking";
        public const string Master = Module + "/tracking/tracking-master";
        public const string Detail = Module + "/tracking/tracking-detail";
        public const string Preview = Module + "/tracking/tracking-preview";
        private const string Default = Rpc + Module + "/tracking";
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
        
        public const string FilterListPlace = Default + "/filter-list-place";
        public const string FilterListPlaceChecking = Default + "/filter-list-place-checking";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListPlace = Default + "/single-list-place";
        public const string SingleListPlaceChecking = Default + "/single-list-place-checking";
        public const string SingleListAppUser = Default + "/single-list-app-user";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(TrackingFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(TrackingFilter.TrackerId), FieldTypeEnum.ID.Id },
            { nameof(TrackingFilter.TargetId), FieldTypeEnum.ID.Id },
            { nameof(TrackingFilter.PlaceId), FieldTypeEnum.ID.Id },
            { nameof(TrackingFilter.PlaceCheckingId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListPlace,FilterListPlaceChecking,FilterListAppUser,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListPlace, SingleListPlaceChecking, SingleListAppUser, 
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
