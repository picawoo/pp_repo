using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Alumni
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите имя")]
    public required string Name { get; set; }
    [Range(1900, 2100, ErrorMessage = "Некорректный год")]
    public int GraduationYear { get; set; }
    public string? Faculty { get; set; }
    public string? CurrentJob { get; set; }
    public List<Mention>? Mentions { get; set; }  // Навигационное свойство
}