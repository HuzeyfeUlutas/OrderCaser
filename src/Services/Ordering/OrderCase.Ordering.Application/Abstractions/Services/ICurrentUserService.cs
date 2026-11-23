using OrderCase.Ordering.Persistence.Abstractions;

namespace OrderCase.Ordering.Application.Abstractions.Services;

public interface ICurrentUserService : IAuditUserProvider
{
    bool IsAdmin { get; }
}