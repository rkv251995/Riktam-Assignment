namespace Assignment.Dtos.Group
{
    public class GroupMessageDto : GroupDto
    {
        public GroupMessageDto()
        {
            Messages = new List<MessageDto>();
        }
        public List<MessageDto> Messages { get; set; }
    }

    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
