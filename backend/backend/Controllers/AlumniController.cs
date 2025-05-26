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
    
    // PUT: api/Alumni/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAlumni(int id, Alumni alumni)
    {
        if (id != alumni.Id) return BadRequest();
        _context.Entry(alumni).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Alumni/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlumni(int id)
    {
        var alumni = await _context.Alumni.FindAsync(id);
        if (alumni == null) return NotFound();
        _context.Alumni.Remove(alumni);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}