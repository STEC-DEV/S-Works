using System.Runtime.CompilerServices;

namespace FamTec.Server.Services
{
    public interface IFileService
    {
        /// <summary>
        /// 새 파일명 생성
        /// </summary>
        /// <returns></returns>
        public string SetNewFileName(string useridx, IFormFile files, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 이미지 등록
        /// </summary>
        /// <param name="folderpath">이미지 등록할 폴더경로</param>
        /// <param name="files">파일 원본</param>
        /// <returns></returns>
        public Task<bool?> AddImageFile(string newFileName, string folderpath, IFormFile files, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 이미지 비율축소 등록
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="folderpath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<bool?> AddResizeImageFile(string newFileName, string folderpath, IFormFile files, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 모바일 Resize
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<byte[]?> AddResizeImageFile_2(IFormFile files, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// PC Resize
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<byte[]?> AddResizeImageFile_3(IFormFile files, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);


        /// <summary>
        /// 이미지 추출
        /// </summary>
        /// <param name="folderpath">이미지 대상 폴더 경로</param>
        /// <param name="filename">대상 파일명</param>
        /// <returns></returns>
        public Task<byte[]?> GetImageFile(string folderpath, string filename, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 이미지 스트림 반환
        /// </summary>
        /// <param name="folderpath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Task<Stream?> GetImageFileStreamAsync(string folderpath, string filename, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);


        /// <summary>
        /// IFormFile 확장자 추출
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string? GetExtension(IFormFile file, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 이미지 삭제
        /// </summary>
        /// <param name="folderPath">이미지 대상 폴더 경로</param>
        /// <param name="filename">삭제할 파일명</param>
        /// <returns></returns>
        public bool DeleteImageFile(string folderPath, string filename, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// byte[] => IFormFile로 변환
        /// </summary>
        /// <param name="Images"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IFormFile? ConvertFormFiles(byte[] Images, string fileName, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary>
        /// 파일존재 유무 확인 - 있으면 true 없으면 false
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool? IsFileExists(string _path, string _fileName, [CallerMemberName] string membername = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0);
    }
}
