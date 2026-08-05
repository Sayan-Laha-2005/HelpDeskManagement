using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Api.Controllers;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HelpDesk.Tests
{
    public class TicketsControllerTests
    {
        private readonly Mock<ITicketRepository> _mockRepo;
        private readonly TicketsController _controller;

        public TicketsControllerTests()
        {
            _mockRepo = new Mock<ITicketRepository>();
            _controller = new TicketsController(_mockRepo.Object);
        }

        private static Ticket SampleTicket(int id = 1) => new Ticket
        {
            Id = id,
            Title = "Laptop won't boot",
            Description = "Blue screen on startup",
            Priority = "High",
            Status = "Open",
            RaisedBy = "john.smith"
        };

        [Fact]
        public async Task GetAllTickets_ReturnsOkWithList()
        {
            _mockRepo.Setup(r => r.GetAllTicketsAsync())
                .ReturnsAsync(new List<Ticket> { SampleTicket() });

            var result = await _controller.GetAllTickets();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tickets = Assert.IsType<List<Ticket>>(okResult.Value);
            Assert.Single(tickets);
        }

        [Fact]
        public async Task GetTicketById_ReturnsOk_WhenFound()
        {
            _mockRepo.Setup(r => r.GetTicketByIdAsync(1)).ReturnsAsync(SampleTicket());

            var result = await _controller.GetTicketById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, ((Ticket)okResult.Value).Id);
        }

        [Fact]
        public async Task GetTicketById_ReturnsNotFound_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetTicketByIdAsync(99)).ReturnsAsync((Ticket)null);

            var result = await _controller.GetTicketById(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTicket_ReturnsCreatedAtAction_WhenValid()
        {
            _mockRepo.Setup(r => r.CreateTicketAsync(It.IsAny<Ticket>())).ReturnsAsync(5);

            var result = await _controller.CreateTicket(SampleTicket());

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TicketsController.GetTicketById), created.ActionName);
        }

        [Fact]
        public async Task CreateTicket_ReturnsBadRequest_WhenPriorityInvalid()
        {
            var ticket = SampleTicket();
            ticket.Priority = "Urgent"; // not a valid value

            var result = await _controller.CreateTicket(ticket);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTicket_ReturnsNoContent_WhenSuccessful()
        {
            var ticket = SampleTicket();
            _mockRepo.Setup(r => r.UpdateTicketAsync(ticket)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateTicket(ticket.Id, ticket);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTicket_ReturnsBadRequest_WhenIdMismatch()
        {
            var ticket = SampleTicket(1);

            var result = await _controller.UpdateTicket(2, ticket);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTicket_ReturnsNoContent_WhenSuccessful()
        {
            _mockRepo.Setup(r => r.DeleteTicketAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteTicket(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetTicketsByStatus_ReturnsBadRequest_ForInvalidStatus()
        {
            var result = await _controller.GetTicketsByStatus("Pending");

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTicketsByStatus_ReturnsOk_ForValidStatus()
        {
            _mockRepo.Setup(r => r.GetTicketsByStatusAsync("Open"))
                .ReturnsAsync(new List<Ticket> { SampleTicket() });

            var result = await _controller.GetTicketsByStatus("Open");

            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
