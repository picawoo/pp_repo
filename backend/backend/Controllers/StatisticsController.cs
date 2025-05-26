using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatisticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("faculties")]
    public async Task<ActionResult<FacultiesStatsDTO>> GetFacultiesStats()
    {
        var stats = await _context.Alumni
            .GroupBy(a => a.Faculty)
            .Select(g => new
            {
                Faculty = g.Key,
                Count = g.Sum(a => a.Mentions.Count)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        return new FacultiesStatsDTO
        {
            Labels = stats.Select(x => x.Faculty).ToArray(),
            Values = stats.Select(x => x.Count).ToArray()
        };
    }
}