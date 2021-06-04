using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Entities
{
    public class Mail : DataEntity
    {
        public long Id { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<Attachment> Attachments { get; set; }
        public long RetryCount { get; set; }
        public string Error { get; set; }
        public Guid RowId { get; set; }
        public Mail() { }
    }

    public class MailFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Recipients { get; set; }
        public StringFilter Subject { get; set; }
        public StringFilter Body { get; set; }
        public LongFilter RetryCount { get; set; }

    }

    public enum MailOrder
    {
        Rece
    }

    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
}
