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
    public class PlaceHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(PlaceHandler);

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
            List<EventMessage<Place>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<Place>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<Place>(context, SyncKey, RowIds);

            List<Place> Places = new List<Place>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Place> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Places.Add(EventMessage.Content);
            }
            try
            {
                List<PlaceDAO> PlaceDAOs = Places.Select(au => new PlaceDAO
                {

                }).ToList();
                await context.BulkMergeAsync(PlaceDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(PlaceHandler));
            }
        }

        private async Task Used(DataContext context, string json)
        {
            try
            {
                List<EventMessage<Place>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<Place>>>(json);
                List<long> PlaceIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
                await context.Place.Where(a => PlaceIds.Contains(a.Id)).UpdateFromQueryAsync(x => new PlaceDAO { Used = true });
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(PlaceHandler));
            }
        }
    }
}
