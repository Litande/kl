namespace Plat4Me.DialLeadProvider.Application.Services.Interfaces;

public interface ILeadsDataSourceSync
{
    Task LeadsSync(CancellationToken ct = default);
}