﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GQL.DAL;
using GQL.DAL.Models;
using GQL.WebApp.Serviced.Infra;
using GraphQL.Execution;
using GraphQl.Server.Annotations;
using GraphQl.Server.Annotations.Attributes;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GQL.WebApp.Serviced.GraphQlV2
{
    [GraphQlType("UsersQuery")]
    public class UsersQuery
    {
        private readonly AppDbContext _appDbContext;


        public UsersQuery(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        [GraphQlField("user")]
        public async Task<UserModelBase> ResolveUserAsync(
            ResolveFieldContext context,
            [GraphQlParameter(Description = "User identificator")] NonNull<string> id,
            [GraphQlParameter(Description = "User type")] UserType? type = null)
        {
            var user = await GetUserModelSet(context, type)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        [GraphQlField("users")]
        public async Task<IEnumerable<UserModelBase>> ResolveUsersAsync(
            ResolveFieldContext context,
            [GraphQlParameter(Description = "User type")] UserType? type = null)
        {
            var users = await GetUserModelSet(context, type)
                .ToListAsync();

            return users;
        }

        [GraphQlField("usersPage")]
        public async Task<Page<UserModelBase>> ResolveUsersPageAsync(
            ResolveFieldContext context,
            [GraphQlParameter(Description = "User type")] UserType? type = null)
        {
            var users = await GetUserModelSet(context, type)
                .ToListAsync();

            return users.AsPage(0, 10, 2, 2, 2);
        }

        [GraphQlField("usersCount")]
        public async Task<int?> ResolveUsersCountAsync(
            ResolveFieldContext context,
            [GraphQlParameter(Description = "Can be 'c' for customer or 'm' for manager.")] string position = null)
        {
            switch (position)
            {
                case "c":
                    return await _appDbContext.Set<CustomerUserModel>().CountAsync();
                case "m":
                    return await _appDbContext.Set<ManagerUserModel>().CountAsync();
                case "":
                case null:
                    return await _appDbContext.Set<UserModelBase>().CountAsync();
                default:
                    context.Errors.Add(new InvalidValueException("type", $"Type '{position}' not found."));
                    return null;
            }
        }

        [GraphQlField("customers")]
        public async Task<IEnumerable<UserModelBase>> ResolveCustomersAsync(ResolveFieldContext context)
        {
            var customers = await GetUserModelSet(context, null)
                .OfType<CustomerUserModel>()
                .ToListAsync();

            return customers;
        }

        private IQueryable<UserModelBase> GetUserModelSet(ResolveFieldContext context, UserType? userType)
        {
            IQueryable<UserModelBase> resultQuery = _appDbContext.Set<UserModelBase>();

            if (context.SubFields.Values.TryFindField(nameof(UserModelBase.Roles), out _))
            {
                resultQuery = resultQuery.Include(u => u.Roles).ThenInclude(r => r.Role);
            }

            if (context.SubFields.Values.TryFindField(nameof(UserModelBase.Friends), out _))
            {
                resultQuery = resultQuery.Include(u => u.Friends).ThenInclude(r => r.Friend);
            }

            if (userType.HasValue)
            {
                resultQuery = resultQuery.Where(u => u.Type == userType.Value);
            }

            return resultQuery;
        }
    }
}