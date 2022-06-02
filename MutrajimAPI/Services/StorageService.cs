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
        //IHostingEnviroment
        #region Property
        private IWebHostEnvironment _hostingEnvironment;
        #endregion

        #region Constructor
        public StorageService(IWebHostEnvironment hostingEnvironment)
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
            }
            stream.Close(); //check
            return files;
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
            var zipName = $"Mutrajim-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        Console.WriteLine(file);
                        var theFile = archive.CreateEntry(file);
                        Console.WriteLine(theFile);
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
        public List<KeyValueModel> Extract(string subDirectory)
        {
            int keyCount = 0, fileCount = 0;
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);
            List<KeyValueModel> TransList = new List<KeyValueModel>();
            string[] projectFiles = Directory.GetFiles(target, "*.json");
            int i = 0;
            foreach (var file in projectFiles)
            {
                fileCount += 1;
                string jsonString = File.ReadAllText(file);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                foreach (var keyValue in dict)
                {
                    KeyValueModel translationModel = new KeyValueModel(i, keyValue.Key, keyValue.Value);
                    TransList.Add(translationModel);
                    //ValList.Add(keyValue.Value);
                    //KeyList.Add(keyValue.Key);
                    //Console.WriteLine(keyValue.Key + ":" + keyValue.Value);
                    //var translation = Translate(keyValue.Value);
                    //dict[keyValue.Key] = translation;

                    keyCount += 1;

                }
                //string serJson = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
                //File.WriteAllText(file, serJson);
            }

            return TransList;
        }
        #endregion

        #region Serialize
        public string Serialize(List<KeyValueModel> translation, string subDirectory)
        {
            string returnVal = "OK!";
            var transList = new Dictionary<string, string>();
            var updatedTrans = translation;
            foreach (var item in updatedTrans)
            {
                transList.Add(item.Key, item.Value);
            }
            string[] paths = { _hostingEnvironment.ContentRootPath, subDirectory, "Updated.json" };
            string fullPath = Path.Combine(paths);
            if (File.Exists(fullPath))
            {
                string serJson = JsonConvert.SerializeObject(transList, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(fullPath, serJson);
            }
            else
            {
                using FileStream newFile = File.Create(fullPath);
                newFile.Close();
                string serJson = JsonConvert.SerializeObject(transList, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(fullPath, serJson);
            }
            return returnVal;
        }
        #endregion

    }
}
