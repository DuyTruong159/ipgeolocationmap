using System.Text.Json;
using ipgeolocationmap.Models;
using Microsoft.AspNetCore.Mvc;

namespace ipgeolocationmap.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    List<IpLocationModel> list = new List<IpLocationModel>();
    public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
    public async Task<IActionResult> Index()
    {
        using(var client = new HttpClient()) {
            client.BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1/?api_key=29654cb0851b4c24b5039be54e79be32");

            HttpResponseMessage response = await client.GetAsync("");

            if (response.IsSuccessStatusCode) {
                string result = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(result);
                var root = jsonDocument.RootElement;

                string ip = root.GetProperty("ip_address").ToString();
                double latitude = root.GetProperty("latitude").GetDouble();
                double longitude = root.GetProperty("longitude").GetDouble();

    
                string data = "https://www.google.com/maps?q=" + latitude.ToString() + "," + longitude.ToString();

                list.Add(new IpLocationModel {Ip = ip, Map = data});

                ViewBag.IPAddress = ip;
                ViewBag.model = list;
                ViewBag.error = false;
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string ipAddress) {
        if (!string.IsNullOrEmpty(ipAddress)) {
            using(var client = new HttpClient()) {
                client.BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1/?api_key=29654cb0851b4c24b5039be54e79be32&ip_address=" + ipAddress);

                HttpResponseMessage response = await client.GetAsync("");

                if (response.IsSuccessStatusCode) {
                    string result = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonDocument.Parse(result);
                    var root = jsonDocument.RootElement;
                    
                    if(root.TryGetProperty("latitude", out JsonElement latitudeElement) && latitudeElement.ValueKind != JsonValueKind.Null) {
                        double latitude = root.GetProperty("latitude").GetDouble();
                        double longitude = root.GetProperty("longitude").GetDouble();
            
                        string data = "https://www.google.com/maps?q=" + latitude.ToString() + "," + longitude.ToString();

                        list.Add(new IpLocationModel {Ip = ipAddress, Map = data});

                        ViewBag.model = list;
                        ViewBag.error = false;
                    } else {
                        ViewBag.error = true;
                    }
                }
            }
        }
        return View("Index");
    }
}
