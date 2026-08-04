using System.Net.Http.Json;
using HelpDesk.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Mvc.Controllers
{
    public class TicketController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ClientName = "HelpDeskApi";

        public TicketController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Ticket
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient(ClientName);
            var tickets = await client.GetFromJsonAsync<List<Ticket>>("api/tickets");
            return View(tickets ?? new List<Ticket>());
        }

        // GET: /Ticket/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient(ClientName);
            var response = await client.GetAsync($"api/tickets/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var ticket = await response.Content.ReadFromJsonAsync<Ticket>();
            return View(ticket);
        }

        // GET: /Ticket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            var client = _httpClientFactory.CreateClient(ClientName);
            var response = await client.PostAsJsonAsync("api/tickets", ticket);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to create the ticket.");
                return View(ticket);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient(ClientName);
            var response = await client.GetAsync($"api/tickets/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var ticket = await response.Content.ReadFromJsonAsync<Ticket>();
            return View(ticket);
        }

        // POST: /Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            var client = _httpClientFactory.CreateClient(ClientName);
            var response = await client.PutAsJsonAsync($"api/tickets/{id}", ticket);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Unable to update the ticket.");
                return View(ticket);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient(ClientName);
            var response = await client.GetAsync($"api/tickets/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var ticket = await response.Content.ReadFromJsonAsync<Ticket>();
            return View(ticket);
        }

        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient(ClientName);
            await client.DeleteAsync($"api/tickets/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
