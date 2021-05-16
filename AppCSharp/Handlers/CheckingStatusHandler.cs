using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Handlers
{
    public class CheckingStatusHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(CheckingStatusHandler);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        //private async Task Sync(DataContext context, string json)
        //{
        //    List<EventMessage<CheckingStatus>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<CheckingStatus>>>(json);
        //    await SaveEventMessage(context, SyncKey, EventMessageReceived);
        //    List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
        //    EventMessageReceived = await ListEventMessage<CheckingStatus>(context, SyncKey, RowIds);

        //    List<CheckingStatus> CheckingStatuses = new List<CheckingStatus>();
        //    foreach (var RowId in RowIds)
        //    {
        //        EventMessage<CheckingStatus> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
        //        if (EventMessage != null)
        //            CheckingStatuses.Add(EventMessage.Content);
        //    }
        //    try
        //    {
        //        List<CheckingStatusDAO> CheckingStatusDAOs = CheckingStatuses.Select(au => new CheckingStatusDAO
        //        {

        //        }).ToList();
        //        await context.BulkMergeAsync(CheckingStatusDAOs);
        //    }
        //    catch (Exception ex)
        //    {
        //        await Log(ex, nameof(CheckingStatusHandler));
        //    }
        //}

    }
}
