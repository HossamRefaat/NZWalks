using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
    public class RegionController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> content = new List<RegionDto>();
            try
            {
                var client = httpClientFactory.CreateClient("NZWalksAPI");

                var httpResponseMessage = await client.GetAsync("https://localhost:7103/api/regions");

                httpResponseMessage.EnsureSuccessStatusCode();

                content.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception)
            {
                throw;
            }

            return View(content);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRegionVM model)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage() 
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7103/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            return (response != null ? RedirectToAction("Index", "Region") : View());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7103/api/regions/{id.ToString()}");

            return response!= null? View(response) : View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7103/api/regions/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();
            
            return response != null? RedirectToAction("Index", "Region") : View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();
            var httpResponseMessage = await client.DeleteAsync($"https://localhost:7103/api/regions/{request.Id}");
            httpResponseMessage.EnsureSuccessStatusCode();
            return RedirectToAction("Index", "Region");
        }
    }
}
