using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Proxies
{
    public interface IUCupProxy
    {
        Task<string> GetTokenAsync(Account loginRequest);
        Task<RecordResponse> Return(VendorRequest request);
        Task<RecordResponse> Rent(VendorRequest request);
    }
}