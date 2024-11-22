using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Infrastructure.Interfaces.Services
{
    public interface IClientCredentialService
    {
        Task<TResponse?> GetAsync<TResponse>(string uri, Dictionary<string, string> headers);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string uri, Dictionary<string, string> headers, TRequest data);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string uri, Dictionary<string, string> headers, TRequest data);
        Task<TResponse?> DeleteAsync<TResponse>(string uri, Dictionary<string, string> headers);
    }
}
