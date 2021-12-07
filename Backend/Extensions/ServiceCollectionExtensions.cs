using HeinjoFood.Api.Data;

namespace HeinjoFood.Api
{
    public static class StorageServiceCollectionExtensions
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.AddSingleton<StorageManager>();
            services.AddSingleton<IFileStorage, BlobFileStorage>();
            return services;
        }
    }
}