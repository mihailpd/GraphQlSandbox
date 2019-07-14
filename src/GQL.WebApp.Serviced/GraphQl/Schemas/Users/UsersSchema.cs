﻿using GQL.WebApp.Serviced.GraphQl.Infra;
using GQL.WebApp.Serviced.GraphQl.Models;
using GQL.WebApp.Serviced.Infra;
using GraphQL.Types;

namespace GQL.WebApp.Serviced.GraphQl.Schemas.Users
{
    public class UsersSchema : Schema
    {
        public UsersSchema(IProvider provider, UsersQuery query, UsersMutation mutation, UsersSubscription subscription)
        {
            //Query = query;
            Query = new ObjectGraphTypeFactory(provider).Create(typeof(GqlUsersQueryType)) as IObjectGraphType;
            Mutation = mutation;
            Subscription = subscription;

            RegisterType<CustomerUserType>();
            RegisterType<ManagerUserType>();
        }
    }
}