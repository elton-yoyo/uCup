using System.Threading.Tasks;
using uCup.Controllers;
using uCup.Models;

namespace uCup.Proxies
{
    public interface IUCupProxy
    {
        Task<string> GetTokenAsync(Account loginRequest);
        Task<RecordResponse> Return(VendorRequest request);
        Task<RecordResponse> Rent(VendorRequest request);
        Task<RecordResponse> Register(RegisterRequest request);
        Task<RecordResponse> RentalStatus(RentalStatusRequest request);
    }
}