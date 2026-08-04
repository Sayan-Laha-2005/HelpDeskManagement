using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET: api/tickets
        [HttpGet]
        public async Task<ActionResult<List<Ticket>>> GetAllTickets()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }
            return Ok(ticket);
        }

        // GET: api/tickets/status/Open
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Ticket>>> GetTicketsByStatus(string status)
        {
            if (!Array.Exists(TicketConstants.ValidStatuses, s => s.Equals(status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest($"Invalid status. Valid values are: {string.Join(", ", TicketConstants.ValidStatuses)}");
            }

            var tickets = await _ticketRepository.GetTicketsByStatusAsync(status);
            return Ok(tickets);
        }

        // POST: api/tickets
        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsValidPriority(ticket.Priority) || !IsValidStatus(ticket.Status))
            {
                return BadRequest("Invalid Priority or Status value.");
            }

            try
            {
                var newId = await _ticketRepository.CreateTicketAsync(ticket);
                ticket.Id = newId;
                return CreatedAtAction(nameof(GetTicketById), new { id = newId }, ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the ticket: {ex.Message}");
            }
        }

        // PUT: api/tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest("Ticket Id in the URL does not match the Id in the request body.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!IsValidPriority(ticket.Priority) || !IsValidStatus(ticket.Status))
            {
                return BadRequest("Invalid Priority or Status value.");
            }

            try
            {
                await _ticketRepository.UpdateTicketAsync(ticket);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the ticket: {ex.Message}");
            }
        }

        // DELETE: api/tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                await _ticketRepository.DeleteTicketAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the ticket: {ex.Message}");
            }
        }

        private static bool IsValidPriority(string priority) =>
            Array.Exists(TicketConstants.ValidPriorities, p => p.Equals(priority, StringComparison.OrdinalIgnoreCase));

        private static bool IsValidStatus(string status) =>
            Array.Exists(TicketConstants.ValidStatuses, s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
    }
}
