using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        // FK to Student
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public Users? Student { get; set; }

        // FK to Course
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        // Navigation
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    }
}