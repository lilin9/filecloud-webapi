namespace Domain
{
    /// <summary>
    /// 统一响应格式
    /// </summary>
    public class ResponseResult
    {
        public bool Success { get; set; }
        public int? Code { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
