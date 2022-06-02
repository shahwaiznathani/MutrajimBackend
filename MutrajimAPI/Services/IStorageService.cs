using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MutrajimAPI.Models
{
    public interface IStorageService
    {
        //void Upload(IFormFile formFile);
        Task<object> UploadFile(IFormFile files, string subDirectory);
        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        string SizeConverter(long bytes);
        List<KeyValueModel> Extract(string subDirectory);
        string Serialize(List<KeyValueModel> translation, string subDirectory);
    }
}
