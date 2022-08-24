using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using MinimalApiRecordTests.Data.Model;
using MinimalApiRecordTests.Model;

namespace MinimalApiRecordTests.Data;

public class UserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Select(x => new User
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName

            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetDuplicateUsers(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .GroupBy(x => new { x.FirstName, x.LastName })
            .Where(x => x.Count() > 1)
            .Select(x => new User
            {
                FirstName = x.Key.FirstName,
                LastName = x.Key.LastName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetUser(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(x => x.Id == id)
            .Select(x => new User
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName

            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>?> FindUserByName(string? firstName, string? lastName, CancellationToken cancellationToken = default)
    {
        if (firstName == null && lastName == null)
            return null;

        //var context = await _contextPool.CreateDbContextAsync(cancellationToken);
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
            .Select(x => new User
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName
            })
            .ToListAsync(cancellationToken);
    }
}
