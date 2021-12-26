﻿using Microsoft.AspNetCore.Http;
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
        #endregion

        #region Constructor
        public FileUploadController(IStorageService fileService)
        {
            _fileService = fileService;
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
        public IActionResult Download([Required] string subDirectory)
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


        //#region extract keys/translate
        //[HttpPost]
        //[Route("extract")]

        //public IActionResult Extract([Required] string subDirectory)
        //{
        //    try
        //    {
        //        var (KeysTranslated, keys) = _fileService.Extract(subDirectory);
        //        return Ok(new { KeysTranslated, keys });


        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //#endregion

    }
}
