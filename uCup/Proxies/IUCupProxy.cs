using System.Threading.Tasks;
using uCup.Models;

namespace uCup.Proxies
{
    public interface IUCupProxy
    {
        public Task<string> GetTokenAsync(Account account);
        Task<RecordResponse> Return(RecordRequest recordRequest, Account account);
    }
}