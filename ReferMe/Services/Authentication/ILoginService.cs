using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReferMe.Models;

namespace ReferMe.Services.Authentication;

public interface ILoginService
{
    ValueTask<LoginResponse?> LoginAsync(LoginRequest login);
    ValueTask<ConnectionInfo?> GetInformationsAsync(string bearerToken = "");
    ValueTask<List<UserInfo>> GetUserInformationsAsync(string bearerToken = "");
}