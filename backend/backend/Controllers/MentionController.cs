using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MentionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MentionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Mentions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mention>>> GetMentions()
    {
        return await _context.Mentions
            .Include(m => m.Alumni)
            .ToListAsync();
    }

    // GET: api/Mentions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Mention>> GetMention(int id)
    {
        var mention = await _context.Mentions
            .Include(m => m.Alumni)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mention == null)
        {
            return NotFound();
        }

        return mention;
    }
    
    // GET: api/Mentions/search?name=Иван
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Mention>>> SearchMentions([FromQuery] string name)
    {
        return await _context.Mentions
            .Include(m => m.Alumni)
            .Where(m => m.Alumni!.Name.Contains(name))
            .ToListAsync();
    }
    
    // GET: api/Mentions/period?start=2024-01-01&end=2024-03-01
    [HttpGet("period")]
    public async Task<ActionResult<IEnumerable<Mention>>> GetMentionsByPeriod(
        [FromQuery] DateTime start, 
        [FromQuery] DateTime end)
    {
        return await _context.Mentions
            .Include(m => m.Alumni)
            .Where(m => m.FoundAt >= start && m.FoundAt <= end)
            .ToListAsync();
    }

    // POST: api/Mentions
    [HttpPost]
    public async Task<ActionResult<Mention>> PostMention(Mention mention)
    {
        _context.Mentions.Add(mention);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMention), new { id = mention.Id }, mention);
    }

    // PUT: api/Mentions/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMention(int id, Mention mention)
    {
        if (id != mention.Id)
        {
            return BadRequest();
        }

        _context.Entry(mention).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MentionExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    private bool MentionExists(int id)
    {
        return _context.Mentions.Any(e => e.Id == id);
    }
    
    // DELETE: api/Mentions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMention(int id)
    {
        var mention = await _context.Mentions.FindAsync(id);
        if (mention == null)
        {
            return NotFound();
        }

        _context.Mentions.Remove(mention);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}