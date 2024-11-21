using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Infrastructure.Interfaces.Repositories;
using AW.Infrastructure.Interfaces.Services;
using AW.Infrastructure.Repositories;
using AW.Infrastructure.Services;

namespace AW.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddHelperServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped(typeof(IBaseSelectRepository<,>), typeof(BaseSelectRepository<,>));
            services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));
            //services.AddScoped(typeof(IBaseAsyncService<,>), typeof(BaseAsyncService<,>));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddEmailServices(configuration);
            //services.AddEmailEndPoint(configuration);
        }

        public static void AddDbContextModel<TDbContext>(this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContext
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<DbContext>(delegate (IServiceProvider serviceProvider, DbContextOptionsBuilder options)
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<TDbContext>(delegate (IServiceProvider serviceProvider, DbContextOptionsBuilder options)
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
