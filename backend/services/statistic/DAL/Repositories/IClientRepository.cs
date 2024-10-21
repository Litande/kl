using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;

namespace Plat4Me.Dial.Statistic.Api.DAL.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAll(CancellationToken ct = default);
}