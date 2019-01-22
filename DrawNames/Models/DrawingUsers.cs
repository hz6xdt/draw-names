using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrawNames.Models
{
    public class DrawingUsers
    {
        public int Id { get; set; }
        [Required]
        public int DrawingId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string DrawnUserId { get; set; }
    }
}
