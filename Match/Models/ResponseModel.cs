namespace Match.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? ResponseCode { get; set; }
    }
}
