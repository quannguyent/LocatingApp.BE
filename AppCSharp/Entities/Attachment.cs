using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Entities
{
    public class Attachment
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public string Extension
        {
            get
            {
                var fileInfo = new FileInfo(FileName);
                return fileInfo.Extension;
            }
        }
    }
}
