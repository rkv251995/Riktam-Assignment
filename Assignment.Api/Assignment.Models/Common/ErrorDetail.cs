using Newtonsoft.Json;

namespace Assignment.Models.Common
{
    public class ErrorDetail : ExceptionDetail
    {
        public ErrorDetail()
        {
            Errors = new();
        }

        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("property")]
        public string Property { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
