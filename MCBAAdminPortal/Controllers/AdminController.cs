using MCBAAdminPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace MCBAAdminPortal.Controllers
{
    public class AdminController : Controller

    {
        private readonly HttpClient _client;

        public AdminController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }
        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            using var response = await _client.GetAsync("api/Customer");
            response.EnsureSuccessStatusCode();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();

            // Deserializing the response received from web api and storing into a list.
            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

            return View(customers);
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            using var response = await _client.GetAsync($"api/Customer/{id}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var customerDto = JsonConvert.DeserializeObject<CustomerDTO>(result); // Deserialize to CustomerDTO

            return View(customerDto); // Pass CustomerDTO to the view
        }

        // POST
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerDTO customerDto) // Use CustomerDTO
        {
            if (id != customerDto.CustomerID)
                return NotFound();

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customerDto), Encoding.UTF8, MediaTypeNames.Application.Json);
                var response = await _client.PutAsync($"api/Customer/{id}", content);

                response.EnsureSuccessStatusCode();
                return RedirectToAction("Customers");
            }

            return View(customerDto);
        }

        [HttpPost("Lock/{loginId}")]
        public async Task<IActionResult> LockAccount(string loginId)
        {

            var response = await _client.PostAsync($"api/Login/lock/{loginId}", null);

            return RedirectToAction("Customers");
        }

        [HttpPost("Unlock/{loginId}")]
        public async Task<IActionResult> UnlockAccount(string loginId)
        {
            var response = await _client.PostAsync($"api/Login/unlock/{loginId}", null);

            return RedirectToAction("Customers");
        }
    }
}
