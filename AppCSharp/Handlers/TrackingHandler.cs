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
    public class TrackingHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(TrackingHandler);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
            if (routingKey == UsedKey)
                await Used(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<Tracking>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<Tracking>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<Tracking>(context, SyncKey, RowIds);

            List<Tracking> Trackings = new List<Tracking>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Tracking> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Trackings.Add(EventMessage.Content);
            }
            try
            {
                List<TrackingDAO> TrackingDAOs = Trackings.Select(au => new TrackingDAO
                {

                }).ToList();
                await context.BulkMergeAsync(TrackingDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(TrackingHandler));
            }
        }

        private async Task Used(DataContext context, string json)
        {
            try
            {
                List<EventMessage<Tracking>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<Tracking>>>(json);
                List<long> TrackingIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
                await context.Tracking.Where(a => TrackingIds.Contains(a.Id)).UpdateFromQueryAsync(x => new TrackingDAO { Used = true });
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(TrackingHandler));
            }
        }
    }
}
