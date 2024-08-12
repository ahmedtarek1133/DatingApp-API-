﻿using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions;

public static class ApplicationServiceExtentions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services 
    , IConfiguration config)
    {
        
        services.AddControllers();
        services.AddDbContext<DataContext>(option =>
            {
                option.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
        services.AddCors();
        services.AddScoped<ITokenService ,TokenService>();
        services.AddScoped<IUserRepository ,UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;

    }
}
