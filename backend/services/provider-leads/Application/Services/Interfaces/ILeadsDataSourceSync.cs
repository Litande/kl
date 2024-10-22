namespace KL.Provider.Leads.Application.Services.Interfaces;

public interface ILeadsDataSourceSync
{
    Task LeadsSync(CancellationToken ct = default);
}