namespace MeasurementPublisherAPI.Services
{
    public interface IMessageConsumer
    {
        void ReadMessage<T>();
    }
}
