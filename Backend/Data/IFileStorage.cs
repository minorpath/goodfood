using System.IO;
using System.Threading.Tasks;

namespace Backend.Data
{
    public interface IFileStorage
    {
        Task<string> Add(string fileName, MemoryStream fileData);
        Task<string> Add(string fileName, string filePath);
    }
}
