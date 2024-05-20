using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReferMe.Models;

namespace ReferMe.Services.Authentication;

internal sealed class LoginService(HttpClient client) : ILoginService
{
    public async ValueTask<LoginResponse?> LoginAsync(LoginRequest login)
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);

            var requestResponse =
                await client.PostAsJsonAsync("https://192.168.11.111:45455/api/Authentication/login", login);

            requestResponse.EnsureSuccessStatusCode();

            var responseJson = await requestResponse.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

            return response;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            return default;
        }
    }

    public async ValueTask<ConnectionInfo?> GetInformationsAsync(string bearerToken = "")
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await client.GetAsync("https://192.168.11.111:45455/api/Authentication/info");

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<Response<UserInfo>>(jsonResponse);

            return new ConnectionInfo { IsConnected = true, User = data.Data };
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return new ConnectionInfo { IsConnected = false };
        }
    }

    public async ValueTask<List<UserInfo>> GetUserInformationsAsync(string bearerToken = "")
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await client.GetAsync("https://192.168.11.111:45455/api/Authentication/infos");

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var responseData = JsonConvert.DeserializeObject<Response<List<UserInfo>>>(jsonResponse);

            return responseData?.Data ?? [];
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return [];
        }
    }
}