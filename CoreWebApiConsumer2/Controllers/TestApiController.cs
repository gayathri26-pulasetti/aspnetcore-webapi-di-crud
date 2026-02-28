using Newtonsoft.Json;
using CoreWebApiConsumer2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CoreWebApiConsumer2.Controllers
{
    public class TestApiController : Controller
    {

        HttpClient client = new HttpClient();
        private readonly IConfiguration _config;
        public TestApiController(IConfiguration config)
        {
            _config = config;
            string? serviceUrl = _config.GetValue<string>("WebApiUrl");
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                client.BaseAddress = new Uri(serviceUrl);
            }
        }

        public async Task<IActionResult> DisplayCustomer(int Custid)  // ✅ Add Custid parameter
        {
            Customer customer = new Customer();  // ✅ Single Customer
            HttpResponseMessage response = await client.GetAsync("Customer/" + Custid);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<Customer>(jsonString) ?? new Customer();
            }
            return View(customer);  // ✅ Send Single Customer
        }

        public async Task<IActionResult> DisplayCustomers()
        {
            List<Customer> customers = new List<Customer>();
            HttpResponseMessage response = await client.GetAsync("Customer");
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<Customer>>(jsonString) ?? new List<Customer>();
            }
            return View(customers);
        }
        public IActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("Customer", customer);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("DisplayCustomers");
            else
                return View();
        }
        public async Task<IActionResult> EditCustomer(int Custid)
        {
            Customer customer = new Customer();
            HttpResponseMessage response = await client.GetAsync("Customer/" + Custid);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                customer = JsonConvert.DeserializeObject<Customer>(result);
            }
            return View(customer);
        }
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync("Customer", customer);
if (response.IsSuccessStatusCode)
return RedirectToAction("DisplayCustomers");
else
return View("EditCustomer");
}
public async Task<IActionResult> DeleteCustomer(int Custid)
        {
            HttpResponseMessage response = await client.DeleteAsync("Customer/" + Custid);
            if (!response.IsSuccessStatusCode)
                ModelState.AddModelError("", "Delete action resulted in an error");
            return RedirectToAction("DisplayCustomers");
        }
    }
}

