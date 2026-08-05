using System;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Api.Data;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HelpDesk.Tests
{
    public class TicketRepositoryTests
    {
        private static HelpDeskDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<HelpDeskDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new HelpDeskDbContext(options);
        }

        private static Ticket SampleTicket() => new Ticket
        {
            Title = "Printer not working",
            Description = "Office printer on 2nd floor is jammed",
            Priority = "Medium",
            Status = "Open",
            RaisedBy = "jane.doe",
            CreatedDate = DateTime.UtcNow
        };

        [Fact]
        public async Task CreateTicketAsync_AddsTicket_AndReturnsGeneratedId()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);

            var id = await repository.CreateTicketAsync(SampleTicket());

            Assert.True(id > 0);
            Assert.Equal(1, await context.Tickets.CountAsync());
        }

        [Fact]
        public async Task GetAllTicketsAsync_ReturnsAllCreatedTickets()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);
            await repository.CreateTicketAsync(SampleTicket());
            await repository.CreateTicketAsync(SampleTicket());

            var tickets = await repository.GetAllTicketsAsync();

            Assert.Equal(2, tickets.Count);
        }

        [Fact]
        public async Task GetTicketByIdAsync_ReturnsCorrectTicket()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);
            var id = await repository.CreateTicketAsync(SampleTicket());

            var ticket = await repository.GetTicketByIdAsync(id);

            Assert.NotNull(ticket);
            Assert.Equal(id, ticket.Id);
        }

        [Fact]
        public async Task GetTicketByIdAsync_ReturnsNull_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);

            var ticket = await repository.GetTicketByIdAsync(999);

            Assert.Null(ticket);
        }

        [Fact]
        public async Task UpdateTicketAsync_UpdatesFields()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);
            var id = await repository.CreateTicketAsync(SampleTicket());

            var updated = new Ticket
            {
                Id = id,
                Title = "Printer fixed",
                Description = "Replaced toner cartridge",
                Priority = "Low",
                Status = "Closed",
                RaisedBy = "jane.doe"
            };
            await repository.UpdateTicketAsync(updated);

            var result = await repository.GetTicketByIdAsync(id);
            Assert.Equal("Printer fixed", result.Title);
            Assert.Equal("Closed", result.Status);
        }

        [Fact]
        public async Task UpdateTicketAsync_ThrowsKeyNotFound_WhenTicketDoesNotExist()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);
            var ticket = SampleTicket();
            ticket.Id = 999;

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(
                () => repository.UpdateTicketAsync(ticket));
        }

        [Fact]
        public async Task DeleteTicketAsync_RemovesTicket()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);
            var id = await repository.CreateTicketAsync(SampleTicket());

            await repository.DeleteTicketAsync(id);

            Assert.Equal(0, await context.Tickets.CountAsync());
        }

        [Fact]
        public async Task DeleteTicketAsync_ThrowsKeyNotFound_WhenTicketDoesNotExist()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(
                () => repository.DeleteTicketAsync(999));
        }

        [Fact]
        public async Task GetTicketsByStatusAsync_ReturnsOnlyMatchingTickets()
        {
            using var context = CreateInMemoryContext();
            var repository = new TicketRepository(context);

            var open = SampleTicket();
            open.Status = "Open";
            var closed = SampleTicket();
            closed.Status = "Closed";

            await repository.CreateTicketAsync(open);
            await repository.CreateTicketAsync(closed);

            var openTickets = await repository.GetTicketsByStatusAsync("Open");

            Assert.Single(openTickets);
            Assert.All(openTickets, t => Assert.Equal("Open", t.Status));
        }
    }
}
