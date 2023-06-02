using System;
using System.Collections.Generic;

namespace Assignment.Infrastructures.EntityFrameworkCore.Entity
{
    public partial class Group
    {
        public Group()
        {
            GroupUserMappings = new HashSet<GroupUserMapping>();
            MessageGroupMappings = new HashSet<MessageGroupMapping>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<GroupUserMapping> GroupUserMappings { get; set; }
        public virtual ICollection<MessageGroupMapping> MessageGroupMappings { get; set; }
    }
}
