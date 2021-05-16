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
    public class PlaceGroupHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(PlaceGroupHandler);

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
            List<EventMessage<PlaceGroup>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<PlaceGroup>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<PlaceGroup>(context, SyncKey, RowIds);

            List<PlaceGroup> PlaceGroups = new List<PlaceGroup>();
            foreach (var RowId in RowIds)
            {
                EventMessage<PlaceGroup> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    PlaceGroups.Add(EventMessage.Content);
            }
            try
            {
                List<PlaceGroupDAO> PlaceGroupDAOs = PlaceGroups.Select(au => new PlaceGroupDAO
                {

                }).ToList();
                await context.BulkMergeAsync(PlaceGroupDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(PlaceGroupHandler));
            }
        }

        private async Task Used(DataContext context, string json)
        {
            try
            {
                List<EventMessage<PlaceGroup>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<PlaceGroup>>>(json);
                List<long> PlaceGroupIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
                await context.PlaceGroup.Where(a => PlaceGroupIds.Contains(a.Id)).UpdateFromQueryAsync(x => new PlaceGroupDAO { Used = true });
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(PlaceGroupHandler));
            }
        }
    }
}
