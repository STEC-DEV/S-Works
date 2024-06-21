namespace FamTec.Shared.Server
{
    public class ResponseObj<T>
    {
        public string message { get; set; }
        public List<T> data { get; set; }
        public int code {  get; set; }

        public ResponseObj() { }
    }
}
