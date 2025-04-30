using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlumniController : ControllerBase
{
    private readonly AppDbContext _context;

    public AlumniController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Alumni
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alumni>>> GetAlumni()
    {
        return await _context.Alumni.ToListAsync();
    }

    // GET: api/Alumni/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Alumni>> GetAlumni(int id)
    {
        var alumni = await _context.Alumni.FindAsync(id);
        if (alumni == null) return NotFound();
        return alumni;
    }

    // POST: api/Alumni
    [HttpPost]
    public async Task<ActionResult<Alumni>> PostAlumni(Alumni alumni)
    {
        _context.Alumni.Add(alumni);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAlumni), new { id = alumni.Id }, alumni);
    }
}