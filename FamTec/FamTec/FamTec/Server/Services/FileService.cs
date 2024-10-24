﻿
using SkiaSharp;

namespace FamTec.Server.Services
{
    public class FileService : IFileService
    {
        private readonly ILogService LogService;
        private readonly ConsoleLogService<FileService> CreateBuilderLogger;

        public FileService(ILogService _logservice,
            ConsoleLogService<FileService> _createbuilderlogger)
        {
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
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
                    await files.CopyToAsync(fileStream).ConfigureAwait(false);
                    return true;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
                // 파일 확장자 결정
                string getExtension = Path.GetExtension(newFileName);
                string targetExtension = getExtension switch
                {
                    ".png" => ".png",
                    ".bmp" => ".bmp",
                    ".jpeg" => ".jpeg",
                    _ => ".png" // 지원되지 않는 확장자는 PNG로 기본설정
                };
                string All_newFileName = Path.ChangeExtension(newFileName, targetExtension);

                // 파일 경로 매핑
                string filePath = Path.Combine(folderpath, All_newFileName);

                using (var memoryStream = new MemoryStream())
                {
                    await files.CopyToAsync(memoryStream).ConfigureAwait(false);
                    memoryStream.Position = 0;

                    using (var originalBitmap = SKBitmap.Decode(memoryStream))
                    {
                        if (originalBitmap == null)
                        {
                            return null;
                        }

                        // 원본 비율 유지하며 크기 조정
                        var resizedBitmap = ResizeBitmapWithAspectRatio(originalBitmap, originalBitmap.Width, originalBitmap.Height);
                        //var resizedBitmap = ResizeBitmapWithAspectRatio(originalBitmap, 300, 300); // 비율유지
                        if (resizedBitmap == null)
                        {
                            return null;
                        }

                        // 이미지 인코딩 형식 결정
                        var encodedFormat = GetEncodedFormat(targetExtension);

                        using (var image = SKImage.FromBitmap(resizedBitmap))
                        using (var data = image.Encode(encodedFormat, 100)) // PNG 퀄리티 무시
                        using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            data.SaveTo(outputStream);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        //public async Task<bool?> AddResizeImageFile(string newFileName, string folderpath, IFormFile files)
        //{
        //    try
        //    {

        //        string getExtension = Path.GetExtension(newFileName); // <- 파일확장자에 맞게 인코딩하기 위함.
        //        string targetExtension = getExtension switch
        //        {
        //            ".png" => ".png",
        //            ".bmp" => ".bmp",
        //            ".jpeg" => ".jpeg",
        //            _ => ".png" // 지원되지 않는 확장자는 PNG로 기본설정
        //        };
        //        string All_newFileName = Path.ChangeExtension(newFileName, targetExtension);

        //        string filePath = Path.Combine(folderpath, All_newFileName); // 파일경로 매핑
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await files.CopyToAsync(memoryStream).ConfigureAwait(false);
        //            memoryStream.Position = 0;


        //            using (var originalBitmap = SKBitmap.Decode(memoryStream))
        //            {
        //                using (var resizedBitmap = originalBitmap.Resize(new SKImageInfo(300,300), SKFilterQuality.High))
        //                {
        //                    if (resizedBitmap == null)
        //                    {
        //                        return null;
        //                    }

        //                    // 이미지 인코딩 형식 결정
        //                    var encodedFormat = GetEncodedFormat(targetExtension);
        //                    using (var image = SKImage.FromBitmap(resizedBitmap))
        //                    using (var data = image.Encode(encodedFormat, 100)) // PNG 퀄리티 무시
        //                    using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //                    {
        //                        data.SaveTo(outputStream);
        //                        return true;
        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.LogMessage(ex.ToString());
        //        return null;
        //    }
        //}

        // 인코딩 형식을 결정
        SKEncodedImageFormat GetEncodedFormat(string extension)
        {
            return extension switch
            {
                ".png" => SKEncodedImageFormat.Png,
                ".bmp" => SKEncodedImageFormat.Bmp,
                ".jpeg" => SKEncodedImageFormat.Jpeg,
                _ => SKEncodedImageFormat.Png // 기본적으로 PNG로 처리
            };
        }

        private SKBitmap ResizeBitmapWithAspectRatio(SKBitmap originalBitmap, int maxWidth, int maxHeight)
        {
            try
            {
                // 원본 이미지의 가로 및 세로 비율 계산
                float widthRatio = (float)maxWidth / originalBitmap.Width;
                float heightRatio = (float)maxHeight / originalBitmap.Height;

                // 더 작은 비율로 축소하여 최대 크기를 넘지 않도록 조정
                float scale = Math.Min(widthRatio, heightRatio);

                // 새 크기 계산 (소수점 반올림)
                int newWidth = (int)Math.Round(originalBitmap.Width * scale);
                int newHeight = (int)Math.Round(originalBitmap.Height * scale);

                // 크기 조정된 새 비트맵 생성
                return originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

                //// 원본 비율 계산
                //float aspectRatio = (float)originalBitmap.Width / originalBitmap.Height;

                //// 목표 크기에 맞춰 조정된 가로, 세로 계산
                //int newWidth = maxWidth;
                //int newHeight = maxHeight;

                //if (aspectRatio > 1)
                //{
                //    // 가로가 더 긴 경우, 가로에 맞추고 세로는 비율에 맞게 조정
                //    newHeight = (int)(maxWidth / aspectRatio);
                //}
                //else
                //{
                //    // 세로가 더 긴 경우, 세로에 맞추고 가로는 비율에 맞게 조정
                //    newWidth = (int)(maxHeight * aspectRatio);
                //}

                //// 크기 조정된 새 비트맵 생성
                //return originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);
            }
            catch (Exception ex)
            {
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
                // 파일 확장자 가져오기 및 소문자로 변환
                string? extension = Path.GetExtension(files.FileName)?.ToLowerInvariant();

                // 기본 확장자는 .png로 설정
                string newExtension = extension switch
                {
                    ".png" => ".png",
                    ".bmp" => ".bmp",
                    ".jpeg" => ".jpeg",
                    _ => ".png" // 지원되지 않는 확장자는 PNG로 기본 설정
                };

                // 새 파일 이름 생성
                string NewFileName = $"{useridx}_{DateTime.Now:yyyyMMddHHmmFFFFFFF}_{files.Name}{newExtension}";
                return NewFileName;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
                // 특정 파일 이름을 가진 파일 검색
                string[] files = Directory.GetFiles(folderpath, filename);

                foreach (string file in files)
                {
                    if (file.Contains(filename))
                    {
                        byte[] ImageBytes = await File.ReadAllBytesAsync(file);
                        return ImageBytes;
                    }
                }

                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
                        //filepath = String.Format(@"{0}\\{1}", folderPath, filename);
                        filepath = Path.Combine(folderPath, filename);
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
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
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }
    
        /// <summary>
        /// FormFile 변환
        /// </summary>
        /// <param name="Images"></param>
        /// <returns></returns>
        public IFormFile? ConvertFormFiles(byte[] Images, string fileName)
        {
            try
            {
                MemoryStream stream = new MemoryStream(Images);
                
                IFormFile? formFile = new FormFile(stream, 0, Images.Length, "files", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png", // 파일 타입에 맞게 수정
                    ContentDisposition = $"form-data; name=\"files\"; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}"
                };

                return formFile;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }

        /// <summary>
        /// 파일존재 유무 확인 - 있으면 true 없으면 false
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool? IsFileExists(string _path, string _fileName)
        {
            try
            {
                string filePath = Path.Combine(_path, _fileName);

                FileInfo fileInfo = new FileInfo(filePath);

                return fileInfo.Exists;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return null;
            }
        }
    }
}
