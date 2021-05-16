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
    public class PlaceGroupRoute : Root
    {
        public const string Parent = Module + "/place-group";
        public const string Master = Module + "/place-group/place-group-master";
        public const string Detail = Module + "/place-group/place-group-detail";
        public const string Preview = Module + "/place-group/place-group-preview";
        private const string Default = Rpc + Module + "/place-group";
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
            { nameof(PlaceGroupFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PlaceGroupFilter.ParentId), FieldTypeEnum.ID.Id },
            { nameof(PlaceGroupFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(PlaceGroupFilter.Code), FieldTypeEnum.STRING.Id },
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
