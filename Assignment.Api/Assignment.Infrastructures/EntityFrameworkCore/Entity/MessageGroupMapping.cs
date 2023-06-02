using System;
using System.Collections.Generic;

namespace Assignment.Infrastructures.EntityFrameworkCore.Entity
{
    public partial class MessageGroupMapping
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid GroupId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual Group Group { get; set; } = null!;
        public virtual Message Message { get; set; } = null!;
    }
}
