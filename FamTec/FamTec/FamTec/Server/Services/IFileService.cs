namespace FamTec.Server.Services
{
    public interface IFileService
    {
        /// <summary>
        /// 새 파일명 생성
        /// </summary>
        /// <returns></returns>
        public string SetNewFileName(string useridx, IFormFile files);

        /// <summary>
        /// 이미지 등록
        /// </summary>
        /// <param name="folderpath">이미지 등록할 폴더경로</param>
        /// <param name="files">파일 원본</param>
        /// <returns></returns>
        public Task<bool?> AddImageFile(string newFileName, string folderpath, IFormFile files);

        /// <summary>
        /// 이밎지 비율축소 등록
        /// </summary>
        /// <param name="newFileName"></param>
        /// <param name="folderpath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Task<bool?> AddResizeImageFile(string newFileName, string folderpath, IFormFile files);

        /// <summary>
        /// 이미지 추출
        /// </summary>
        /// <param name="folderpath">이미지 대상 폴더 경로</param>
        /// <param name="filename">대상 파일명</param>
        /// <returns></returns>
        public Task<byte[]?> GetImageFile(string folderpath, string filename);

        /// <summary>
        /// IFormFile 확장자 추출
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string? GetExtension(IFormFile file);

        /// <summary>
        /// 이미지 삭제
        /// </summary>
        /// <param name="folderPath">이미지 대상 폴더 경로</param>
        /// <param name="filename">삭제할 파일명</param>
        /// <returns></returns>
        public bool DeleteImageFile(string folderPath, string filename);

    }
}
