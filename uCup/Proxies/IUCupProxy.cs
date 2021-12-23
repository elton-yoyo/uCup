using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Proxies
{
    public interface IUCupProxy
    {
        public Task<string> GetTokenAsync(LoginRequest loginRequest);
        Task<RecordResponse> Return(VendorRequest request);
        Task<RecordResponse> Rent(VendorRequest request);
    }
}