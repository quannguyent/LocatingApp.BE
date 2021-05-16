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
    public class PlaceCheckingHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(PlaceCheckingHandler);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<PlaceChecking>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<PlaceChecking>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<PlaceChecking>(context, SyncKey, RowIds);

            List<PlaceChecking> PlaceCheckings = new List<PlaceChecking>();
            foreach (var RowId in RowIds)
            {
                EventMessage<PlaceChecking> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    PlaceCheckings.Add(EventMessage.Content);
            }
            try
            {
                List<PlaceCheckingDAO> PlaceCheckingDAOs = PlaceCheckings.Select(au => new PlaceCheckingDAO
                {

                }).ToList();
                await context.BulkMergeAsync(PlaceCheckingDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(PlaceCheckingHandler));
            }
        }

    }
}
