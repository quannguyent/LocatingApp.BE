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
    public class AppUserHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(AppUserHandler);

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
            List<EventMessage<AppUser>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            EventMessageReceived = await ListEventMessage<AppUser>(context, SyncKey, RowIds);

            List<AppUser> AppUsers = new List<AppUser>();
            foreach (var RowId in RowIds)
            {
                EventMessage<AppUser> EventMessage = EventMessageReceived.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    AppUsers.Add(EventMessage.Content);
            }
            try
            {
                List<AppUserDAO> AppUserDAOs = AppUsers.Select(au => new AppUserDAO
                {

                }).ToList();
                await context.BulkMergeAsync(AppUserDAOs);
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(AppUserHandler));
            }
        }

        private async Task Used(DataContext context, string json)
        {
            try
            {
                List<EventMessage<AppUser>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);
                List<long> AppUserIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
                await context.AppUser.Where(a => AppUserIds.Contains(a.Id)).UpdateFromQueryAsync(x => new AppUserDAO { Used = true });
            }
            catch (Exception ex)
            {
                await Log(ex, nameof(AppUserHandler));
            }
        }
    }
}
