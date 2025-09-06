using Microsoft.AspNetCore.Http;

namespace ZarmallStore.Application.Extention
{
    public static class UploadFileExtension
    {
        public static bool AddFileToServer(this IFormFile file, string fileName, string orginalPath, string deletefileName = null)
        {
            if (file != null)
            {
                if (!Directory.Exists(orginalPath))
                    Directory.CreateDirectory(orginalPath);

                if (!string.IsNullOrEmpty(deletefileName))
                {
                    if (File.Exists(orginalPath + deletefileName))
                        File.Delete(orginalPath + deletefileName);
                }

                string OriginPath = orginalPath + fileName;

                using (var stream = new FileStream(OriginPath, FileMode.Create))
                {
                    if (!Directory.Exists(OriginPath)) file.CopyTo(stream);
                }

                return true;
            }

            return false;
        }

        public static void DeleteFile(this string fileName, string OriginPath)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (File.Exists(OriginPath + fileName))
                    File.Delete(OriginPath + fileName);
            }
        }
    }
}
