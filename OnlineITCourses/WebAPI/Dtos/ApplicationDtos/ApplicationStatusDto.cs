using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.ApplicationDtos
{
    public class ApplicationStatusDto
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
