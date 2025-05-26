using backend.Data;
using backend.DTOs;
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
        return await _context.Mentions.Include(m => m.Alumni).ToListAsync();
    }

    // GET: api/Mentions/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Mention>> GetMention(int id)
    {
        var mention = await _context.Mentions.Include(m => m.Alumni).FirstOrDefaultAsync(m => m.Id == id);
        if (mention == null) return NotFound();
        return mention;
    }
    
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<MentionDTO>>> GetLatestMentions()
    {
        return await _context.Mentions
            .OrderByDescending(m => m.FoundAt)
            .Take(4)
            .Select(m => new MentionDTO
            {
                Title = m.Content.Length > 50 
                    ? m.Content.Substring(0, 50) + "..." 
                    : m.Content,
                Source = new Uri(m.SourceUrl).Host.Replace("www.", ""),
                SourceUrl = m.SourceUrl,
                FoundAt = m.FoundAt
            })
            .ToListAsync();
    }

    // POST: api/Mentions
    [HttpPost]
    public async Task<ActionResult<Mention>> PostMention(
        [FromBody] CreateMentionDTO mentionDto)
    {
        var alumniExists = await _context.Alumni.AnyAsync(a => a.Id == mentionDto.AlumniId);
        if (!alumniExists)
        {
            return BadRequest("Alumni not found");
        }

        var mention = new Mention
        {
            SourceUrl = mentionDto.SourceUrl,
            Content = mentionDto.Content,
            AlumniId = mentionDto.AlumniId,
            FoundAt = DateTime.UtcNow
        };

        _context.Mentions.Add(mention);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMention), new { id = mention.Id }, mention);
    }

    // PUT: api/Mentions/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMention(int id, Mention mention)
    {
        if (id != mention.Id) return BadRequest();
        _context.Entry(mention).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/Mentions/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMention(int id)
    {
        var mention = await _context.Mentions.FindAsync(id);
        if (mention == null) return NotFound();
        _context.Mentions.Remove(mention);
        await _context.SaveChangesAsync();
        return NoContent();
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
}