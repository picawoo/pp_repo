namespace backend.Models;

public class Mention
{
    public int Id { get; set; }
    public required string SourceUrl { get; set; }
    public required string Content { get; set; }
    public DateTime FoundAt { get; set; } = DateTime.UtcNow;
    public int AlumniId { get; set; }
    public Alumni? Alumni { get; set; }
}