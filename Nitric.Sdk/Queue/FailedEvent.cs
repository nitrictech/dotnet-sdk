namespace Nitric.Api.Queue
{
    public class FailedEvent : Common.Event
    {
        public string Message { get; set; }
        public FailedEvent(Common.Event nitricEvent, string message) :
        base(nitricEvent.RequestId, nitricEvent.PayloadType, nitricEvent.PayloadStruct)
        {
            Message = message;
        }
    }
}
