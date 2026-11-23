namespace OrderCase.Ordering.Persistence.Abstractions;

public interface IAuditUserProvider
{
    string UserId { get; }
}