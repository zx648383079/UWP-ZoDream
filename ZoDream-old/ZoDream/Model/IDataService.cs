using System.Threading.Tasks;

namespace ZoDream.Model
{
    public interface IDataService
    {
        Task<DataItem> GetData();
    }
}