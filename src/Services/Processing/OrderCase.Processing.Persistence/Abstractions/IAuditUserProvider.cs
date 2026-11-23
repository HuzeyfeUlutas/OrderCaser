namespace OrderCase.Processing.Persistence.Abstractions;

public interface IAuditUserProvider
{
    string UserId { get; }
}