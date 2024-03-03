﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DashcamVideoArchive.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultDashcamServices(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(builder =>
            {
                builder.ConfigureHttpClient((serviceProvider, httpClient) =>
                {
                    var endpoint = serviceProvider.GetRequiredService<IConfiguration>()["DASHCAM_ENDPOINT"]
                    ?? throw new InvalidOperationException("Missing DASHCAM_ENDPOINT value.");

                    httpClient.BaseAddress = new Uri(endpoint);
                });
            });

            services.AddTransient<IFileDownloader, FileDownloader>();
            services.AddTransient<IDashcamArchiver, DashcamArchiver>();

            return services;
        }
    }
}
