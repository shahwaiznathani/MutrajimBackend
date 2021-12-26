using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MutrajimAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly MutrajimDbContext _context;
        private readonly IStorageService _fileService;

        public TranslationController(MutrajimDbContext context, IStorageService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/Translation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TranslationModel>>> GetTranslatios()
        {
            return await _context.Translations.ToListAsync();
        }

        // GET: api/Translation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TranslationModel>> GetTranslation(int id)
        {
            var translation = await _context.Translations.FindAsync(id);

            if (translation == null)
            {
                return NotFound();
            }

            return translation;
        }

        // PUT: api/Translation/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTranslation(int id, TranslationModel translation)
        {
            if (id != translation.KeyID)
            {
                return BadRequest();
            }

            _context.Entry(translation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TranslationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Translation
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TranslationModel>> PostTranslation(TranslationModel translation)
        {
            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTranslation", new { id = translation.KeyID }, translation);
        }

        //Post File Data into Database
        [HttpPost]
        [Route("NewKeyValue")]
        public async Task<ActionResult<TranslationModel>> PostFileData(string SubDirectory)
        {
            var FileData = _fileService.Extract(SubDirectory);
            foreach(TranslationModel data in FileData)
            {
                Console.WriteLine(data.KeyID + data.Key + ":" + data.Value);
                await PostTranslation(data);
            }
            return Ok(FileData);
        }

        // DELETE: api/Translation/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TranslationModel>> DeleteTranslation(int id)
        {
            var translation = await _context.Translations.FindAsync(id);
            if (translation == null)
            {
                return NotFound();
            }

            _context.Translations.Remove(translation);
            await _context.SaveChangesAsync();

            return translation;
        }

        private bool TranslationExists(int id)
        {
            return _context.Translations.Any(e => e.KeyID == id);
        }
    }
}
