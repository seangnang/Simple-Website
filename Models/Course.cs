using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Thumbnail { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsPublished { get; set; } = false;
        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK to Instructor
        public string InstructorId { get; set; } = string.Empty;

        [ForeignKey("InstructorId")]
        public Users? Instructor { get; set; }

        [ForeignKey("CategoryId")]
        public Categories? Category { get; set; }

        // Navigation
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}