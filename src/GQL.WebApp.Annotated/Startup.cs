﻿using GQL.DAL;
using GQL.WebApp.Annotated.Managers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GQL.WebApp.Annotated
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;


        public Startup(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(o => o.AllowSynchronousIO = true);

            var isDev = _environment.IsDevelopment();

            services.AddSingleton<IUsersObservable, UsersObservable>();
            services.AddScoped<IUsersManager, UsersManager>();

            //services.AddGraphQl<GraphQlSchema>(c => c
            //    .RegisterObject<UsersQuery>()
            //    .RegisterInterface<IUserObject>(mc => mc
            //        .Map<UserModelBase>())
            //    .RegisterObject<CustomerUserObject>(mc => mc
            //        .Map<CustomerUserModel>())
            //    .RegisterObject<ManagerUserObject>(mc => mc
            //        .Map<ManagerUserModel>())
            //    .RegisterObject<UsersMutation>()
            //    .RegisterInputObject<CustomerInputObject>()
            //    .RegisterInputObject<ManagerInputObject>());

            //services.AddMvc();
            //services.AddDbContext<AppDbContext>(b => b.UseInMemoryDatabase(nameof(AppDbContext)));

            //services
            //    .AddGraphQL(o => o.EnableMetrics = o.ExposeExceptions = isDev)
            //    .AddWebSockets();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseGraphQl();

            //app.UseWebSockets();

            //app.UseGraphQL<GraphQlSchema>();
            //app.UseGraphQLWebSockets<GraphQlSchema>();

            //app.UseGraphQLPlayground(new GraphQLPlaygroundOptions { GraphQLEndPoint = PathString.FromUriComponent("/graphql") });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
            DbSeeder.Seed(dbContext);
        }
    }
}
