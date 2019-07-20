﻿using System;
using System.Reflection;
using GQL.WebApp.Serviced.Infra;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GQL.WebApp.Serviced.GraphQl.Infra.Resolvers
{
    public class PropertyFieldResolver : IFieldResolver
    {
        private readonly Type _targetType;
        private readonly PropertyInfo _propertyInfo;
        private readonly IProvider _provider;


        public PropertyFieldResolver(Type targetType, PropertyInfo propertyInfo, IProvider provider)
        {
            _targetType = targetType;
            _propertyInfo = propertyInfo;
            _provider = provider;
        }


        public object Resolve(ResolveFieldContext context)
        {
            var target = context.Source ?? _provider.Get(_targetType);
            var value = _propertyInfo.GetValue(target);

            return value;
        }
    }
}