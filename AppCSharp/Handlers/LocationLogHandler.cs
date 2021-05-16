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
    public class LocationLogHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(LocationLogHandler);

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
            List<EventMessage<LocationLog>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<LocationLog>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<LocationLog>(context, SyncKey, RowIds);

            List<LocationLog> LocationLogs = new List<LocationLog>();
            foreach (var RowId in RowIds)
            {
                EventMessage<LocationLog> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    LocationLogs.Add(EventMessage.Content);
            }
            try
            {
                List<LocationLogDAO> LocationLogDAOs = LocationLogs.Select(au => new LocationLogDAO
                {

                }).ToList();
                await context.BulkMergeAsync(LocationLogDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(LocationLogHandler));
            }
        }

        private async Task Used(DataContext context, string json)
        {
            try
            {
                List<EventMessage<LocationLog>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<LocationLog>>>(json);
                List<long> LocationLogIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
                await context.LocationLog.Where(a => LocationLogIds.Contains(a.Id)).UpdateFromQueryAsync(x => new LocationLogDAO { Used = true });
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(LocationLogHandler));
            }
        }
    }
}
