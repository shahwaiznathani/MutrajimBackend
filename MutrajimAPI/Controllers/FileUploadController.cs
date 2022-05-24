using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<FileSetting>> PostFileDetails(FileSetting file)
        {
            _context.FileSettings.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id = file.fileID }, file);
        }

    }
}
