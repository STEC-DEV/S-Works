namespace FamTec.Server.Services
{
    public interface IFileService
    {
        public Task<string?> AddImageFile(string folderpath, IFormFile files);

        public Task<byte[]?> GetImageFile(string folderpath, string filename);

        public bool DeleteImageFile(string folderPath, string filename);
    }
}
