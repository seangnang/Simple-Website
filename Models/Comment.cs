using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }
    }
}