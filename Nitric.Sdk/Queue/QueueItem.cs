namespace Nitric.Api.Queue
{
    public class QueueItem : Common.Event
    {
        public string LeaseID { get; set; }
        public QueueItem(Common.Event nitricEvent, string leaseID) :
        base(nitricEvent.RequestId, nitricEvent.PayloadType, nitricEvent.PayloadStruct)
        {
            LeaseID = leaseID;
        }
    }
}
