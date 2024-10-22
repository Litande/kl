using KL.Statistics.Application.Models.Entities;

namespace KL.Statistics.DAL.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAll(CancellationToken ct = default);
}