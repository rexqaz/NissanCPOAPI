using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Http;
using Azure.Storage.Blobs;
using System.Configuration;

namespace WebApplication.Services
{
    public class FileService
    {       
        public FileService()
        {
            
        }

        /// <summary>
        /// 儲存到Azure Blob
        /// </summary>
        /// <param name="file">圖檔</param>
        /// <param name="folderPath">額外資料夾</param>
        /// <param name="fileHeaderChar">新檔名前置字元</param>
        /// <returns>回傳檔案完整路徑</returns>
        public string SaveAsBlob(HttpPostedFileBase file, string folderPath, string fileHeaderChar = "")
        {
            if (file == null || file.ContentLength == 0)
            {
                return "";
            }
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new Exception("請帶入上傳資料夾名稱");
            }
          
            var savePath = GetSavePath(folderPath, file);
            var uniqueName = CreateUniqueName(file, fileHeaderChar);

            // 建立 Blob 服務客戶端
            string BlobStorageConnectionString = ConfigurationManager.AppSettings["blob"];
            var blobServiceClient = new BlobServiceClient(BlobStorageConnectionString);

            // 建立容器客戶端
            var myFilesContainer = blobServiceClient.GetBlobContainerClient("cpo-storage");
            // 建立 Blob 客戶端
            BlobClient myFileBlob = myFilesContainer.GetBlobClient($"/{savePath}/{uniqueName}");

            Stream fileStream = file.InputStream;
            fileStream.Position = 0;
            var result = myFileBlob.Upload(fileStream, true);
            var httpStatusCode = result.GetRawResponse().Status;
            if (httpStatusCode >= 200 && httpStatusCode <= 299)
            {
                return $"cpo-storage/{savePath}/{uniqueName}";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 儲存到Azure Blob
        /// </summary>
        /// <param name="file">圖檔</param>
        /// <param name="folderPath">額外資料夾</param>
        /// <param name="fileHeaderChar">新檔名前置字元</param>
        /// <returns>回傳檔案完整路徑</returns>
        public string SaveAsBlob(MultipartFileData file, string folderPath, string fileHeaderChar = "")
        {
            if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
            {
                return "";
            }

            if (string.IsNullOrEmpty(folderPath))
            {
                throw new Exception("請帶入上傳資料夾名稱");
            }

            var savePath = GetSavePath(folderPath, file);
            var uniqueName = CreateUniqueName(file, fileHeaderChar);

            // 建立 Blob 服務客戶端
            string BlobStorageConnectionString = ConfigurationManager.AppSettings["blob"];
            var blobServiceClient = new BlobServiceClient(BlobStorageConnectionString);

            // 建立容器客戶端
            var myFilesContainer = blobServiceClient.GetBlobContainerClient("cpo-storage");
            // 建立 Blob 客戶端
            BlobClient myFileBlob = myFilesContainer.GetBlobClient($"/{savePath}/{uniqueName}");

            Stream fileStream = File.OpenRead(file.LocalFileName);
            if (fileStream != null)
            {
                using (fileStream)
                {
                    fileStream.Position = 0;
                    var result = myFileBlob.Upload(fileStream, true);
                    var httpStatusCode = result.GetRawResponse().Status;
                    if (httpStatusCode >= 200 && httpStatusCode <= 299)
                    {
                        return $"cpo-storage/{savePath}/{uniqueName}";
                    }
                }
            }

            return "";
        }
        /// <summary>
        /// 刪除 azure blob 檔案
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteBlob(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            // 建立 Blob 服務客戶端
            string BlobStorageConnectionString = ConfigurationManager.AppSettings["blob"];
            var blobServiceClient = new BlobServiceClient(BlobStorageConnectionString);

            // 建立容器客戶端
            var myFilesContainer = blobServiceClient.GetBlobContainerClient("cpo-storage");
            // 建立 Blob 客戶端
            //db filename - cpo-storage/test/file/CarSell/230824021112145_8b94a9jpg
            fileName = fileName.Replace("cpo-storage", "");
            BlobClient myFileBlob = myFilesContainer.GetBlobClient($"{fileName}");

            try
            {
                if (!myFileBlob.Exists())
                {
                    return true;
                }
                myFileBlob.DeleteIfExists();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 檔案實際路徑(blob)
        /// </summary>
        /// <param name="blobFilePath"></param>
        /// <returns></returns>
        public string GetRealUrl(string blobFilePath)
        {
            string blobPath = ConfigurationManager.AppSettings["imgPath"];
            if (string.IsNullOrEmpty(blobFilePath))
            {
                return "";
            }
            else
            {
                return blobPath + blobFilePath;
            }
        }

        /// <summary>
        /// 建立唯一檔名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileHeaderChar">新檔名前置字元</param>
        /// <returns></returns>
        public string CreateUniqueName(MultipartFileData file, string fileHeaderChar = "")
        {
            var fileExtension = GetFileExtension(file.Headers.ContentDisposition.FileName.Trim('\"'));
            var defaultName = CreateDefaultFileName(fileExtension);

            if (string.IsNullOrEmpty(fileHeaderChar))
                return defaultName;
            else
                return $"{fileHeaderChar}_{defaultName}";
        }

        /// <summary>
        /// 建立唯一檔名
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileHeaderChar">新檔名前置字元</param>
        /// <returns></returns>
        public string CreateUniqueName(HttpPostedFileBase file, string fileHeaderChar = "")
        {            
            var fileExtension = GetFileExtension(file);
            var defaultName = CreateDefaultFileName(fileExtension);

            if (string.IsNullOrEmpty(fileHeaderChar))
                return defaultName;
            else
                return $"{fileHeaderChar}_{defaultName}";
        }

        private string GetSavePath(string folderPath, HttpPostedFileBase file)
        {
            string folderRoot = ConfigurationManager.AppSettings["imgPath2"];
            string folderType = string.Empty;
            string fileExtension = GetFileExtension(file);

            if (IsImage(fileExtension))
            {
                folderType = "image";              
            }
            else
            {
                folderType = "file";
            }

            return $"{folderRoot}/{folderType}/{folderPath}";
        }
        private string GetSavePath(string folderPath, string fileName)
        {
            string folderRoot = ConfigurationManager.AppSettings["imgPath2"];
            string folderType = string.Empty;
            string fileExtension = Path.GetExtension(fileName);

            if (IsImage(fileExtension))
            {
                folderType = "image";
            }
            else
            {
                folderType = "file";
            }

            return $"{folderRoot}/{folderType}/{folderPath}";
        }

        private string GetSavePath(string folderPath, MultipartFileData file)
        {
            string folderRoot = ConfigurationManager.AppSettings["imgPath2"];
            string folderType = string.Empty;
            string fileExtension = GetFileExtension(file);

            if (IsImage(fileExtension))
            {
                folderType = "image";
            }
            else
            {
                folderType = "file";
            }

            return $"{folderRoot}/{folderType}/{folderPath}";
        }

        private string CreateDefaultFileName(string fileExtension)
        {
            if (!fileExtension.Contains("."))
            {
                fileExtension = "." + fileExtension;
            }
            return $"{DateTime.Now:yyMMddHHmmssfff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{fileExtension}";
        }

        private string GetFileName(HttpPostedFileBase file)
        {
            return Path.GetFileName(file.FileName);
        }

        private string GetFileExtension(HttpPostedFileBase file)
        {
            return GetFileExtension(file.FileName);
        }

        private string GetFileExtension(MultipartFileData file)
        {
            return GetFileExtension(file.Headers.ContentDisposition.FileName.Replace("\\", "").Replace("\"", ""));
        }

        private string GetFileExtension(string filename)
        {
            int dotIndex = filename.LastIndexOf('.');
            if (dotIndex > 0 && dotIndex < filename.Length - 1)
            {
                return filename.Substring(dotIndex + 1).ToLower();
            }
            return "";
        }

        private bool IsImage(string fileExtension)
        {
            // 定義常見的圖片副檔名
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            fileExtension = fileExtension.ToLower();
            // 判斷副檔名是否在圖片副檔名列表中
            return Array.Exists(imageExtensions, ext => ext == fileExtension);
        }
    }
}