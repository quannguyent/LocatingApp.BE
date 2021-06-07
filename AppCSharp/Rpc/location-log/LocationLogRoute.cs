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
using LocatingApp.Services.MLocationLog;
using LocatingApp.Services.MAppUser;

namespace LocatingApp.Rpc.location_log
{
    public class LocationLogRoute : Root
    {
        public const string Parent = Module + "/location-log";
        public const string Master = Module + "/location-log/location-log-master";
        public const string Detail = Module + "/location-log/location-log-detail";
        public const string Preview = Module + "/location-log/location-log-preview";
        private const string Default = Rpc + Module + "/location-log";
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
        public const string GetCurrentLocation = Default + "/get-current-location";
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListLocationLog = Default + "/filter-list-location-log";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListLocationLog = Default + "/single-list-location-log";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(LocationLogFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(LocationLogFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(LocationLogFilter.Latitude), FieldTypeEnum.DECIMAL.Id },
            { nameof(LocationLogFilter.Longtitude), FieldTypeEnum.DECIMAL.Id },
            { nameof(LocationLogFilter.UpdateInterval), FieldTypeEnum.LONG.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListLocationLog,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListLocationLog, 
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
