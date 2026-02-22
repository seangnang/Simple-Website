using System.ComponentModel.DataAnnotations;

namespace SimpleWebsite.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = "ti ti-category"; // Tabler icon

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}