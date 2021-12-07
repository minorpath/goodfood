namespace HeinjoFood.Api.Data
{
    public interface IFileStorage
    {
        Task<string> Add(string fileName, MemoryStream fileData);
        Task<string> Add(string fileName, string filePath);
        Task<Stream?> GetAsync(string fileName);
        string? GetContentType(string fileName);
    }
}
