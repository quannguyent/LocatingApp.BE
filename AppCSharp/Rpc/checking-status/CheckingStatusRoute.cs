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
using LocatingApp.Services.MCheckingStatus;

namespace LocatingApp.Rpc.checking_status
{
    public class CheckingStatusRoute : Root
    {
        public const string Parent = Module + "/checking-status";
        public const string Master = Module + "/checking-status/checking-status-master";
        public const string Detail = Module + "/checking-status/checking-status-detail";
        public const string Preview = Module + "/checking-status/checking-status-preview";
        private const string Default = Rpc + Module + "/checking-status";
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
        



        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CheckingStatusFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CheckingStatusFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(CheckingStatusFilter.Name), FieldTypeEnum.STRING.Id },
        };

        private static List<string> FilterList = new List<string> { 
            
        };
        private static List<string> SingleList = new List<string> { 
            
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "T??m ki???m", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Th??m", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "S???a", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xo??", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xo?? nhi???u", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xu???t excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nh???p excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
