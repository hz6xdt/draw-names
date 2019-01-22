using System.ComponentModel.DataAnnotations;

namespace DrawNames.Models
{
    public class Drawing
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool Drawn { get; set; }
    }
}
