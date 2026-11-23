namespace OrderCase.Processing.Application.Abstractions.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken)
        where TEvent : class;
}