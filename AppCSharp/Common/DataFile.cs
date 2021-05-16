using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Common
{
    public class DataFile
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Stream Content { get; set; }
    }
}
