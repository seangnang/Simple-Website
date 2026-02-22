using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class LessonProgress
    {
        [Key]
        public int LessonProgressId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? WatchedAt { get; set; }

        // FK to Enrollment
        public int EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment? Enrollment { get; set; }

        // FK to Lesson
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }
    }
}