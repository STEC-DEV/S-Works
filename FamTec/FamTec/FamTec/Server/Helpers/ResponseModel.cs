namespace FamTec.Server.Helpers
{
    public class ResponseModel<T>
    {
        public string? message { get; set; }
        public T? data { get; set; }
        public int? code { get; set; }
    }
}
