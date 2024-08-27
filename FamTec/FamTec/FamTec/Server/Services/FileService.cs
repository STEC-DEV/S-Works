using DocumentFormat.OpenXml.Packaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FamTec.Server.Services
{
    public class FileService : IFileService
    {
        private readonly ILogService LogService;
        public FileService(ILogService _logservice)
        {
            this.LogService = _logservice;
        }


        /// <summary>
        /// 이미지 등록
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool?> AddImageFile(string newFileName, string folderpath, IFormFile files)
        {
            try
            {
                string filePath = Path.Combine(folderpath, newFileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await files.CopyToAsync(fileStream);
                    return true;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }
        
        /// <summary>
        /// 이미지 비율 축소 등록
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="folderpath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool?> AddResizeImageFile(string newFileName, string folderpath, IFormFile files)
        {
            try
            {
                string filePath = Path.Combine(folderpath, newFileName);
                using (var memoryStream = new MemoryStream())
                {
                    await files.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using (Image image = await Image.LoadAsync(memoryStream))
                    {
                        image.Mutate(x => x.Resize(300, 300));
                        image.Save(filePath);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 새로운 파일명 생성
        /// </summary>
        /// <returns></returns>
        public string SetNewFileName(string useridx, IFormFile files)
        {
            try
            {
                string NewFileName = $"{useridx}_{DateTime.Now.ToString("yyyyMMddHHmmFFFFFFF")}_{files.Name}{Path.GetExtension(files.FileName)}";
                return NewFileName;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 이미지 추출
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<byte[]?> GetImageFile(string folderpath, string filename)
        {
            try
            {
                string[] FileList = Directory.GetFiles(folderpath);

                if (FileList is [_, ..])
                {
                    foreach (var file in FileList)
                    {
                        if (file.Contains(filename))
                        {
                            byte[] ImageBytes = await File.ReadAllBytesAsync(file);
                            return ImageBytes;
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 이미지 삭제
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool DeleteImageFile(string folderPath, string filename)
        {
            try
            {
                string filepath = String.Empty;
                DirectoryInfo di = new DirectoryInfo(folderPath);
                if (di.Exists)
                {
                    if (!String.IsNullOrWhiteSpace(filename))
                    {
                        filepath = String.Format(@"{0}\\{1}", folderPath, filename);
                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 확장자 추출
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string? GetExtension(IFormFile file)
        {
            try
            {
                return Path.GetExtension(file.FileName);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }

        public Task<string?> AddImageFile(string folderpath, IFormFile files)
        {
            throw new NotImplementedException();
        }
    }
}
