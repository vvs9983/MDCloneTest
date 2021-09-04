using System.Data;
using System.Threading.Tasks;
using Test.Interfaces;

namespace Test.Services
{
    public class CsvFileService : ICsvFileService
    {
        public async Task<DataTable> LoadCsvFileAsync(string path) => await Task.Run(() => CSVLibraryAK.Core.CSVLibraryAK.Import(path, true));
    }
}
