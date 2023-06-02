namespace Assignment.Dtos.Common
{
    public class CommonDto<T> 
    {
        public CommonDto()
        {
            Response = default;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Response { get; set; }
    }
}
