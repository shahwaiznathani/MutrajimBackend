using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MutrajimAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace MutrajimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly AuthenticationContext _context;
        private readonly IStorageService _fileService;

        public TranslationController(AuthenticationContext context, IStorageService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: api/Translation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KeyValueModel>>> GetTranslatios()
        {
            return await _context.Translations.ToListAsync();
        }

        // GET: api/Translation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KeyValueModel>> GetTranslation(int id)
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
        public async Task<IActionResult> PutTranslation(int id, KeyValueModel translation)
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
        public async Task<ActionResult<KeyValueModel>> PostTranslation(KeyValueModel translation)
        {
            _context.Translations.Add(translation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTranslation", new { id = translation.KeyID }, translation);
        }

        //Post File Data into Database
        [HttpPost]
        [Route("NewKeyValue")]
        public async Task<ActionResult<KeyValueModel>> PostFileData(string SubDirectory)
        {
            var FileData = _fileService.Extract(SubDirectory);
            foreach(KeyValueModel data in FileData)
            {
                try
                {
                    await PostTranslation(data);
                }
                //Console.WriteLine(data.KeyID + data.Key + ":" + data.Value);
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            return Ok(FileData); //check
        }

        //Delete the whole table once the File is downloaded
        [HttpDelete]
        [Route ("DeleteTable")]
        public async Task<ActionResult<IEnumerable<KeyValueModel>>> DeleteAll()
        {
            string sqlTrunc = "TRUNCATE TABLE " + "Translations";
            SqlConnection connec = new SqlConnection("Server=DESKTOP-2JRO2KQ;Database=Identity-fyp-DB;Trusted_Connection=True;MultipleActiveResultSets=true");
            connec.Open();
            SqlCommand cmd = new SqlCommand(sqlTrunc, connec);
            cmd.ExecuteNonQuery();
            connec.Close();

            //Delete all files from directory
            System.IO.DirectoryInfo di = new DirectoryInfo("FileStorage");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            return await _context.Translations.ToListAsync();
        }

        // DELETE: api/Translation/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KeyValueModel>> DeleteTranslation(int id)
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

        // GET: api/Translation/Serialize
        [HttpGet]
        [Route("Serialize")]
        public async Task<ActionResult<string>> SerializeTranslations(string subDirectory)
        {
            var translation = await _context.Translations.ToListAsync();
            var FileData = _fileService.Serialize(translation, subDirectory);
            return Ok(FileData);
        }
        private bool TranslationExists(int id)
        {
            return _context.Translations.Any(e => e.KeyID == id);
        }
    }
}
