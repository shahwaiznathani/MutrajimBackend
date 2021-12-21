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
        void UploadFile(List<IFormFile> files, string subDirectory);
        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        string SizeConverter(long bytes);
        (string, List<string>) Extract(string subDirectory);

        string Translate(string word);
    }
}
