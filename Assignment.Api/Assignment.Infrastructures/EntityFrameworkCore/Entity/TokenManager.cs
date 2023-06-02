using System;
using System.Collections.Generic;

namespace Assignment.Infrastructures.EntityFrameworkCore.Entity
{
    public partial class TokenManager
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiredOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
