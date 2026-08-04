using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Api.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Valid values: Low, Medium, High
        [Required]
        [MaxLength(20)]
        public string Priority { get; set; }

        // Valid values: Open, In Progress, Closed
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        [Required]
        [MaxLength(100)]
        public string RaisedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
