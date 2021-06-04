using LocatingApp.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace LocatingApp.Entities
{
    public class Image : DataEntity, IEquatable<Image>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public byte[] Content { get; set; }

        public bool Equals(Image other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
