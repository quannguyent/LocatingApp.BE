using LocatingApp.Common;
using LocatingApp.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace LocatingApp.Handlers
{
    public class EventMessage<T> : DataEntity
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
        public string EntityName { get; set; }
        public T Content { get; set; }

        public EventMessage() { }
        public EventMessage(T content, Guid RowId)
        {
            Time = StaticParams.DateTimeNow;
            this.RowId = RowId;
            EntityName = typeof(T).Name;
            this.Content = content;
        }
    }

    public class EventMessageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public DateFilter Time { get; set; }
        public GuidFilter RowId { get; set; }
        public StringFilter EntityName { get; set; }
        public EventMessageOrder OrderBy { get; set; }
        public EventMessageSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventMessageOrder
    {
        Id = 0,
        Time = 1,
        RowId = 2,
        EntityName = 3,
    }

    [Flags]
    public enum EventMessageSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Time = E._1,
        RowId = E._2,
        EntityName = E._3
    }
}
