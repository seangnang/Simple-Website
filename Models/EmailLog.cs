using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class EmailLog
    {
        [Key]
        public int EmailLogId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [ForeignKey("UserId")]
        public Users? User { get; set; }
    }
}