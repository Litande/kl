using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Application.Models.Requests;
using Plat4Me.DialAgentApi.Application.Models.Responses;

namespace Plat4Me.DialAgentApi.Persistent.Repositories;

public abstract class RepositoryBase
{
    public virtual async Task<PaginatedResponse<T>> CreatePaginatedResponse<T>(
        IQueryable<T> query,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = totalCount > 0
            ? await ApplyPagination(query, pagination)
                .ToArrayAsync(cancellationToken)
            : Enumerable.Empty<T>();

        return new PaginatedResponse<T>
        {
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            Items = items,
        };
    }

    public virtual IQueryable<T> ApplyPagination<T>(
        IQueryable<T> query,
        PaginationRequest pagination)
    {
        if (pagination.SortBy is not null)
        {
            var propInfo = typeof(T).GetProperty(
                pagination.SortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (propInfo is null)
                throw new InvalidDataException($"Field '{pagination.SortBy}' not found in {typeof(T).FullName}");

            query = pagination.SortDirection is SortDirectionTypes.Asc
                ? query.OrderBy(r => EF.Property<object>(r!, propInfo.Name))
                : query.OrderByDescending(r => EF.Property<object>(r!, propInfo.Name));
        }

        query = query
            .Skip(pagination.PageSize * (pagination.Page - 1))
            .Take(pagination.PageSize);

        return query;
    }

    public virtual PaginatedResponse<T> CreatePaginatedResponse<T>(
        ICollection<T> query,
        PaginationRequest pagination)
    {
        var totalCount = query.Count;

        var items = totalCount > 0
            ? ApplyPagination(query, pagination)
                .ToArray()
            : Enumerable.Empty<T>();

        return new PaginatedResponse<T>
        {
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            Items = items,
        };
    }

    public virtual IEnumerable<T> ApplyPagination<T>(
        IEnumerable<T> query,
        PaginationRequest pagination)
    {
        if (pagination.SortBy is not null)
        {
            var propInfo = typeof(T).GetProperty(
                pagination.SortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (propInfo is null)
                throw new InvalidDataException($"Field '{pagination.SortBy}' not found in {typeof(T).FullName}");

            query = pagination.SortDirection is SortDirectionTypes.Asc
                ? query.OrderBy(x => propInfo.GetValue(x, null))
                : query.OrderByDescending(x => propInfo.GetValue(x, null));
        }

        query = query
            .Skip(pagination.PageSize * (pagination.Page - 1))
            .Take(pagination.PageSize);

        return query;
    }

    public string FullSearchPattern(string pattern)
    {
        return $"%{pattern}%";
    }
}
