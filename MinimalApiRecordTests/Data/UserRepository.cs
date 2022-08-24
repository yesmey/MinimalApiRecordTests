using Microsoft.EntityFrameworkCore;
using MinimalApiRecordTests.Data.Model;
using MinimalApiRecordTests.Internal;
using MinimalApiRecordTests.Models;
using System.Linq.Expressions;

namespace MinimalApiRecordTests.Data;

public class UserRepository
{
    private readonly DataContext _context;

    private static readonly Expression<Func<UserData, User>> MapUserData =
        (UserData userData) => new User
        {
            Id = userData.Id,
            FirstName = userData.FirstName,
            LastName = userData.LastName
        };

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAll(RangeFilter range, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Select(MapUserData)
            .ApplyRange(range)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUser(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(x => x.Id == id)
            .Select(MapUserData)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>?> FindUserByName(string? firstName, string? lastName, RangeFilter range, CancellationToken cancellationToken = default)
    {
        if (firstName == null && lastName == null)
            return null;

        IQueryable<UserData> queryable = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(firstName))
        {
            queryable = queryable.Where(x => x.FirstName == firstName);
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            queryable = queryable.Where(x => x.LastName == lastName);
        }

        return await queryable
            .Select(MapUserData)
            .ApplyRange(range)
            .ToListAsync(cancellationToken);
    }
}
