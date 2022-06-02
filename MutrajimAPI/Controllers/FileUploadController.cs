using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MutrajimAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        #region Property
        private readonly IStorageService _fileService;
        private readonly AuthenticationContext _context;
        #endregion

        #region Constructor
        public FileUploadController(IStorageService fileService, AuthenticationContext context)
        {
            _fileService = fileService;
            _context = context;
        }
        #endregion

        #region Upload
        [HttpPost]
        [Route("upload")]
        public async Task<ActionResult> Upload()
        {
            //parameter post-body form
            try
            {
                var files = HttpContext.Request.Form.Files[0];
                string subDirectory = Directory.GetCurrentDirectory() + "/FileStorage";
                Console.WriteLine(files.FileName);
                await _fileService.UploadFile(files, subDirectory);
                Console.WriteLine(files.FileName + " " + files.GetType());
                return Ok(new { files.ContentType});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Download File
        [HttpGet]
        [Route("download")]
        public IActionResult Download(string subDirectory)
        {

            try
            {
                var (fileType, archiveData, archiveName) = _fileService.DownloadFiles(subDirectory);
                
                return File(archiveData, fileType, archiveName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        //GET FILE DETAILS BY ID FOR TRANSLATION PAGE
        [HttpGet("{id}")]
        public async Task<ActionResult<FileSetting>> GetFile(int id)
        {
            var file = await _context.FileSettings.FindAsync(id);

            if (file == null)
            {
                return NotFound();
            }

            return file;
        }
        //POST FILE DETAILS ON FILE UPLOAD
        [HttpPost]
        [Route("PostFileSetting")]
        public async Task<ActionResult<FileSetting>> PostFileDetails(FileSettingDTO dto)
        {
            FileSetting file = new FileSetting();
            file.fileLocation = Directory.GetCurrentDirectory() + "/FileStorage/" + dto.name;
            file.fileFormat = dto.type;
            _context.FileSettings.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id = file.fileID }, file);
        }

        [HttpDelete]
        [Route("DeleteTable")]
        public async Task<ActionResult<IEnumerable<KeyValueModel>>> DeleteAll()
        {
            string sqlTrunc = "TRUNCATE TABLE " + "FileSettings";
            SqlConnection connec = new SqlConnection("Server=DESKTOP-2JRO2KQ;Database=Identity-fyp-DB;Trusted_Connection=True;MultipleActiveResultSets=true");
            connec.Open();
            SqlCommand cmd = new SqlCommand(sqlTrunc, connec);
            cmd.ExecuteNonQuery();
            connec.Close();
            return await _context.Translations.ToListAsync();
        }

    }
}
