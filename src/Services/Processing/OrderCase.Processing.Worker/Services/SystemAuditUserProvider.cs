using OrderCase.Processing.Persistence.Abstractions;

namespace OrderCase.Processing.Worker.Services;

public sealed class SystemAuditUserProvider : IAuditUserProvider
{
    public string UserId => "system";
}