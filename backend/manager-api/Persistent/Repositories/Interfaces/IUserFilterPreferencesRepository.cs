using KL.Manager.API.Application.Models.Requests.UserFilter;
using KL.Manager.API.Application.Models.Responses.UserFilter;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

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