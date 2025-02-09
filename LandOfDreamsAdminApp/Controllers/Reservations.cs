using GemBox.Document;
using LandOfDreamsAdminApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System.Text;

namespace LandOfDreamsAdminApp.Controllers
{
    public class Reservations : Controller
    {
        public Reservations()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public async Task<IActionResult> Index()
        {
            HttpClient client = new HttpClient();
            string URL = "http://localhost:5095/api/Admin/GetAllReservations";

            
            HttpResponseMessage response = await client.GetAsync(URL);

            if (!response.IsSuccessStatusCode)
            {
                
                throw new Exception("Failed to retrieve data from the API. Status code: " + response.StatusCode);
            }

            
            var data = await response.Content.ReadAsAsync<List<Reservation>>();

            return View(data);
        }

        public IActionResult Details(Guid Id)
        {
            HttpClient client = new HttpClient();
            string URL = "http://localhost:5095/api/Admin/GetDetails";
            var model = new
            {
                Id = Id
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Reservation>().Result;
            return View(data);
        }
        public FileContentResult CreateInvoice(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"http://localhost:5095/api/Admin/GetDetails/{id}";  

                HttpResponseMessage response = client.GetAsync(url).Result;  

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = response.Content.ReadAsStringAsync().Result;
                    throw new Exception($"Error from API: {response.StatusCode}, {errorContent}");
                }

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var reservation = JsonConvert.DeserializeObject<Reservation>(jsonString);  

                if (reservation == null)
                {
                    throw new Exception("Reservation not found.");
                }

                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.odt");
                var document = DocumentModel.Load(templatePath);

                
                document.Content.Replace("{{BookingDate}}", reservation.BookingDate?.ToString("yyyy-MM-dd") ?? "N/A");
                document.Content.Replace("{{TravelEndDate}}", reservation.TravelEndDate?.ToString("yyyy-MM-dd"));
                document.Content.Replace("{{NumberOfPeople}}", reservation.NumberOfPeople?.ToString());
                document.Content.Replace("{{TotalCost}}", reservation.TotalCost?.ToString("C"));
                document.Content.Replace("{{FirstName}}", reservation.FirstName);
                document.Content.Replace("{{LastName}}", reservation.LastName);
                document.Content.Replace("{{Email}}", reservation.Email);
                document.Content.Replace("{{PhoneNumber}}", $"{reservation.PhoneNumberPrefix} {reservation.PhoneNumber}"); 

                
                using (var stream = new MemoryStream())
                {
                    document.Save(stream, new PdfSaveOptions());
                    return File(stream.ToArray(), "application/pdf", "ExportedInvoice.pdf");
                }
            }
        }





    }
}
