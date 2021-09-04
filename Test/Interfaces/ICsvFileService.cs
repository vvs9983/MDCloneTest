using System.Data;
using System.Threading.Tasks;

namespace Test.Interfaces
{
    public interface ICsvFileService
    {
        public Task<DataTable> LoadCsvFileAsync(string path);
    }
}
