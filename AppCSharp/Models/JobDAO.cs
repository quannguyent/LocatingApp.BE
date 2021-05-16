using System;
using System.Collections.Generic;

namespace LocatingApp.Models
{
    public partial class JobDAO
    {
        public JobDAO()
        {
            JobParameters = new HashSet<JobParameterDAO>();
            States = new HashSet<StateDAO>();
        }

        public long Id { get; set; }
        public long? StateId { get; set; }
        public string StateName { get; set; }
        public string InvocationData { get; set; }
        public string Arguments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }

        public virtual ICollection<JobParameterDAO> JobParameters { get; set; }
        public virtual ICollection<StateDAO> States { get; set; }
    }
}
