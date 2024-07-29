using Octopus.Data.ApiResponses;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Octopus.Data;

public class OctopusApiClient : HttpClient
{
    public OctopusApiClient(string apiKey, string apiUrl)
    {
        BaseAddress = new Uri(apiUrl);
        var keyBytes = Encoding.UTF8.GetBytes(apiKey);

        DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(keyBytes));
    }

    public async ValueTask<Account> GetAccount(string accountNumber)
    {
        Console.WriteLine("Retrieving Account Information ...");

        Console.WriteLine($"Octopus GET: {BaseAddress}v1/accounts/{accountNumber}/");
        HttpResponseMessage result = await GetAsync($"v1/accounts/{accountNumber}/");

        var account = await result.Content.ReadFromJsonAsync<Account>();

        Console.WriteLine("Retrieving Account Information Complete!");

        return account;
    }

    public async ValueTask<string> GetProduct(string productCode)
    {
        Console.WriteLine($"Octopus GET: {BaseAddress}v1/products/{productCode}/");
        HttpResponseMessage result = await GetAsync($"v1/products/{productCode}/");
        return await result.Content.ReadAsStringAsync();
    }

    public async ValueTask<OctopusDataResponse<Charge>> GetStandingCharges(string type, string productCode, string tariffCode)
    {
        Console.WriteLine($"Octopus GET: {BaseAddress}v1/products/{productCode}/{type}-tariffs/{tariffCode}/standing-charges/?page_size=20000");
        HttpResponseMessage result = await GetAsync($"v1/products/{productCode}/{type}-tariffs/{tariffCode}/standing-charges/?page_size=20000");
        return await result.Content.ReadFromJsonAsync<OctopusDataResponse<Charge>>();
    }

    public async ValueTask<OctopusDataResponse<Rate>> GetUnitRates(string type, string productCode, string tariffCode)
    {
        Console.WriteLine($"Octopus GET: {BaseAddress}v1/products/{productCode}/{type}-tariffs/{tariffCode}/standard-unit-rates/?page_size=20000");
        HttpResponseMessage result = await GetAsync($"v1/products/{productCode}/{type}-tariffs/{tariffCode}/standard-unit-rates/?page_size=20000");
        return await result.Content.ReadFromJsonAsync<OctopusDataResponse<Rate>>();
    }

    public async ValueTask<OctopusDataResponse<ConsumptionItem>> GetConsumption(string type, string mprn, string serial_number)
    {
        Console.WriteLine($"Octopus GET: {BaseAddress}v1/{type}-meter-points/{mprn}/meters/{serial_number}/consumption/?page_size=20000");
        HttpResponseMessage result = await GetAsync($"v1/{type}-meter-points/{mprn}/meters/{serial_number}/consumption/?page_size=20000");
        return await result.Content.ReadFromJsonAsync<OctopusDataResponse<ConsumptionItem>>();
    }

    public async ValueTask<T> Get<T>(string fullUrl)
    {
        var relativeUrl = fullUrl.Replace(BaseAddress.AbsoluteUri.ToString(), "");

        Console.WriteLine($"Octopus GET: {BaseAddress}{relativeUrl}");

        var result = await GetAsync(relativeUrl);
        return await result.Content.ReadFromJsonAsync<T>();
    }
}