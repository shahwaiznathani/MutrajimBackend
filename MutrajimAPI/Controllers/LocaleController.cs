using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MutrajimAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocaleController : ControllerBase
    {
        private readonly AuthenticationContext _context;

        public LocaleController(AuthenticationContext context)
        {
            _context = context;
        }

        // GET: api/Locale
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocaleSetting>>> GetProjects()
        {
            return await _context.LocaleSettings.ToListAsync();
        }

        // GET: api/Locale/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocaleSetting>> GetProject(int id)
        {
            var project = await _context.LocaleSettings.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Locale/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, LocaleSetting project)
        {
            if (id != project.settingId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Locale
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<LocaleSetting>> PostProject(LocaleSetting project)
        {
            _context.LocaleSettings.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.settingId }, project);
        }

        // DELETE: api/Locale/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LocaleSetting>> DeleteProject(int id)
        {
            var project = await _context.LocaleSettings.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.LocaleSettings.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        [HttpDelete]
        [Route("DeleteTable")]
        public async Task<ActionResult<IEnumerable<KeyValueModel>>> DeleteAll()
        {
            string sqlTrunc = "TRUNCATE TABLE " + "LocaleSettings";
            SqlConnection connec = new SqlConnection("Server=DESKTOP-2JRO2KQ;Database=Identity-fyp-DB;Trusted_Connection=True;MultipleActiveResultSets=true");
            connec.Open();
            SqlCommand cmd = new SqlCommand(sqlTrunc, connec);
            cmd.ExecuteNonQuery();
            connec.Close();
            return await _context.Translations.ToListAsync();
        }

        private bool ProjectExists(int id)
        {
            return _context.LocaleSettings.Any(e => e.settingId == id);
        }
    }
}
