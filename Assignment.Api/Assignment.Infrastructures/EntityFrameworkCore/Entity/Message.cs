using System;
using System.Collections.Generic;

namespace Assignment.Infrastructures.EntityFrameworkCore.Entity
{
    public partial class Message
    {
        public Message()
        {
            MessageGroupMappings = new HashSet<MessageGroupMapping>();
        }

        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<MessageGroupMapping> MessageGroupMappings { get; set; }
    }
}
