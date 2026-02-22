using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWebsite.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string VideoUrl { get; set; } = string.Empty;

        public int Duration { get; set; } // in minutes

        public int Order { get; set; }

        public bool IsPreview { get; set; } = false;

        // FK to Course
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        // Navigation
        public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    }
}