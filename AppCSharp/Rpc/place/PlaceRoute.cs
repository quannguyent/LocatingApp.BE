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
    public class PlaceRoute : Root
    {
        public const string Parent = Module + "/place";
        public const string Master = Module + "/place/place-master";
        public const string Detail = Module + "/place/place-detail";
        public const string Preview = Module + "/place/place-preview";
        private const string Default = Rpc + Module + "/place";
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
        
        public const string FilterListPlaceGroup = Default + "/filter-list-place-group";

        public const string SingleListPlaceGroup = Default + "/single-list-place-group";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(PlaceFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PlaceFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(PlaceFilter.PlaceGroupId), FieldTypeEnum.ID.Id },
            { nameof(PlaceFilter.Radius), FieldTypeEnum.LONG.Id },
            { nameof(PlaceFilter.Latitude), FieldTypeEnum.DECIMAL.Id },
            { nameof(PlaceFilter.Longtitude), FieldTypeEnum.DECIMAL.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListPlaceGroup,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListPlaceGroup, 
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
