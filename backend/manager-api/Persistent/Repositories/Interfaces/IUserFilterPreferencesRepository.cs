using Plat4Me.DialClientApi.Application.Models.Requests.UserFilter;
using Plat4Me.DialClientApi.Application.Models.Responses.UserFilter;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface IUserFilterPreferencesRepository
{
    Task<IReadOnlyCollection<UserFilterResponse>> GetUserFilters(long userId, CancellationToken ct = default);

    Task AddFilter(
        long userId, CreateUserFilterRequest request,
        CancellationToken ct = default);

    Task UpdateFilter(
        long userId, UpdateUserFilterRequest request,
        CancellationToken ct = default);

    Task DeleteFilter(long userId, long filterId, CancellationToken ct = default);
}