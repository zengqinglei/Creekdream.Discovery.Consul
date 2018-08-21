using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebApplication1
{
    /// <summary>
    /// 服务发现测试
    /// </summary>
    public class DiscoveryConsulTest : WebApplicationFactory<Startup>
    {
        private HttpClient _httpClient;

        public DiscoveryConsulTest(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Test_Get_Health()
        {
            var response = await _httpClient.GetAsync("api/Health");
            response.IsSuccessStatusCode.ShouldBe(true);
        }

        [Fact]
        public async Task Test_Get_Service()
        {
            var serviceJsonStr = await _httpClient.GetStringAsync("");
            serviceJsonStr.ShouldContain("ConsulTestService");
        }
    }
}
