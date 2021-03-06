﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using RandomWebApi.Configuration;

namespace RandomWebApi
{
    public class Startup
    {
        private const string AllowCustomSpecificOrigin = "AllowCustomSpecificOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string apiUrl = Configuration.GetSection("Frontend:Url").Get<string>();

            services.AddCors(options => {
                options.AddPolicy(AllowCustomSpecificOrigin, 
                builder => {
                    builder.WithOrigins(apiUrl).AllowAnyHeader().AllowAnyMethod();
                });
            });

            var databaseConfig = Configuration.GetSection("Database").Get<DatabaseConfiguration>();

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
            connectionStringBuilder.Host = databaseConfig.Server;
            connectionStringBuilder.Username = databaseConfig.User;
            connectionStringBuilder.Password = databaseConfig.Password;
            connectionStringBuilder.Database = databaseConfig.DatabaseName;

            services.AddDbContext<RandomDataProject.AppContext>(options => {
                options.UseNpgsql(connectionStringBuilder.ConnectionString);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(AllowCustomSpecificOrigin);

            app.UseMvc();
        }
    }
}
