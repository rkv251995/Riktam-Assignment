namespace Assignment.Dtos.Group
{
    public class GroupMemberDto : GroupDto
    {
        public GroupMemberDto()
        {
            Members = new List<MemberDto>();
        }
        public List<MemberDto> Members { get; set; }
    }

    public class MemberDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
