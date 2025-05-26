using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class CreateMentionDTO
{
    [Required]
    public string SourceUrl { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public int AlumniId { get; set; }
}