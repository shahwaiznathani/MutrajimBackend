using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MutrajimAPI.Models
{
    public class StorageService:IStorageService
    {
        #region Property
        private IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructor
        public StorageService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Upload File
        public async Task<object> UploadFile( IFormFile files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);

            Directory.CreateDirectory(target);
            //if (files.Length <= 0);
            var filePath = Path.Combine(target, files.FileName);
            var stream = new FileStream(filePath, FileMode.Create);
            {
                await files.CopyToAsync(stream);
                return files;
            }
            //files.ForEach(async file =>
            //{
            //    if (file.Length <= 0) return;
            //    var filePath = Path.Combine(target, file.FileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(stream);
            //    }
            //});
        }
        #endregion

        #region Download File
        public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }

        }
        #endregion

        #region Size Converter
        public string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }
        #endregion

        #region Extract Keys
        public List<TranslationModel> Extract(string subDirectory)
        {
            int keyCount = 0, fileCount = 0;
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);
            List<TranslationModel> TransList = new List<TranslationModel>();
            string[] projectFiles = Directory.GetFiles(target, "*.json");
            int i = 0;
            foreach (var file in projectFiles)
            {
                fileCount += 1;
                string jsonString = File.ReadAllText(file);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                foreach (var keyValue in dict)
                {
                    TranslationModel translationModel = new TranslationModel(i, keyValue.Key, keyValue.Value);
                    TransList.Add(translationModel);
                    //ValList.Add(keyValue.Value);
                    //KeyList.Add(keyValue.Key);
                    //Console.WriteLine(keyValue.Key + ":" + keyValue.Value);
                    var translation = Translate(keyValue.Value);
                    dict[keyValue.Key] = translation;

                    keyCount += 1;

                }
                string serJson = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(file, serJson);
            }

            return TransList;
        }
        #endregion

        #region Translate key
        public string Translate(string translation)
        {
            return translation;
        }
        #endregion

    }
}
