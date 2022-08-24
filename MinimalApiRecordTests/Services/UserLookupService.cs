﻿using MinimalApiRecordTests.Data;
using MinimalApiRecordTests.Models;
using static MinimalApiRecordTests.Services.UserLookupResult;

namespace MinimalApiRecordTests.Services;

// Poor man's discriminated union :)
public record UserLookupResult
{
    private UserLookupResult() { }
    public record FoundOne(User User) : UserLookupResult;
    public record FoundMany(IReadOnlyList<User> Users) : UserLookupResult;
    public record NotFound() : UserLookupResult;
    public record Error(string ErrorMessage, Exception? Exception = null) : UserLookupResult;
}

public class UserLookupService
{
    private readonly UserRepository _userRepository;

    public UserLookupService(UserRepository userRepository) => _userRepository = userRepository;

    public async Task<UserLookupResult> GetAllUsers(RangeFilter range, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userRepository.GetAll(range, cancellationToken) switch
            {
                { Count: > 0 } users => new FoundMany(users),
                { } => new NotFound()
            };
        }
        catch (Exception ex)
        {
            return new Error("Unknown error", ex);
        }
    }

    public async Task<UserLookupResult> FindUser(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userRepository.GetUser(id, cancellationToken) switch
            {
                { Count: 1 } users => new FoundOne(users[0]),
                { Count: > 1 } => new Error("Duplicate item found"),
                { } => new NotFound()
            };
        }
        catch (Exception ex)
        {
            return new Error("Unknown error", ex);
        }
    }

    public async Task<UserLookupResult> SearchUser(string? firstName, string? lastName, RangeFilter range, CancellationToken cancellationToken = default)
    {
        try
        {
            firstName = firstName?.Trim();
            lastName = lastName?.Trim();

            return await _userRepository.FindUserByName(firstName, lastName, range, cancellationToken) switch
            {
                { Count: 1 } users => new FoundOne(users[0]),
                { Count: > 1 } users => new FoundMany(users),
                { } => new NotFound(),
                null => new Error("Invalid search criteria")
            };
        }
        catch (Exception ex)
        {
            return new Error("Unknown error", ex);
        }
    }
}

public static class UserLookupServiceExtensions
{
    public static void MapUserLookupEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", GetAllUsers).WithTags(nameof(UserLookupService)).WithName(nameof(GetAllUsers));
        app.MapGet("/users/{id}", GetUser).WithTags(nameof(UserLookupService)).WithName(nameof(GetUser));
        app.MapGet("/users/search", SearchUser).WithTags(nameof(UserLookupService)).WithName(nameof(SearchUser));

        static async Task<IResult> GetAllUsers([AsParameters] RangeFilter range, UserLookupService userService, CancellationToken cancellationToken) =>
            await userService.GetAllUsers(range, cancellationToken) switch
            {
                FoundMany { Users: var users } => Results.Ok(users),
                NotFound => Results.NotFound(),
                Error error => Results.BadRequest(error.ErrorMessage),
                { } or null => Results.StatusCode(StatusCodes.Status501NotImplemented)
            };

        static async Task<IResult> GetUser(int id, UserLookupService userService, CancellationToken cancellationToken) =>
            await userService.FindUser(id, cancellationToken) switch
            {
                FoundOne { User: var user } => Results.Ok(user),
                NotFound => Results.NotFound(),
                Error error => Results.BadRequest(error.ErrorMessage),
                { } or null => Results.StatusCode(StatusCodes.Status501NotImplemented)
            };

        static async Task<IResult> SearchUser(string? firstName, string? lastName, [AsParameters] RangeFilter range, UserLookupService userService, CancellationToken cancellationToken) =>
            await userService.SearchUser(firstName, lastName, range, cancellationToken) switch
            {
                FoundOne { User: var user } => Results.Ok(user),
                FoundMany { Users: var users } => Results.Ok(users),
                NotFound => Results.NotFound(),
                Error error => Results.BadRequest(error.ErrorMessage),
                { } or null => Results.StatusCode(StatusCodes.Status501NotImplemented)
            };
    }
}
