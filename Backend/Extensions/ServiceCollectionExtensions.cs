using Backend.Data;

namespace Backend
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