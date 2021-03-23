namespace Nitric.Api.Faas
{
    public class Context
    {
        public string RequestId { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string PayloadType { get; set; }
        public Context(string requestId, string source, string sourceType, string payloadType)
        {
            RequestId = requestId;
            Source = source;
            SourceType = sourceType;
            PayloadType = payloadType;
        }
        public static string CleanHeader(string headerName)
        {
            return headerName.ToLower().Replace("x-nitric-", "").Replace("-", "_");
        }
    }
}
