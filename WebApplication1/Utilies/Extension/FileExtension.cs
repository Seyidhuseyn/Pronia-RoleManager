namespace WebApplication1.Utilies.Extension
{
    public static class FileExtension
    {
        public static bool CheckType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
        public static bool CheckSize(this IFormFile file, int kb)
        {
            return kb * 1024 > file.Length;
        }
        public static string SaveFile(this IFormFile file, string path)
        {
            string fileName = ChangeFileName(file.FileName);
            using (FileStream fs = new FileStream(Path.Combine(path,fileName),FileMode.Create))
            {
                file.CopyTo(fs);
            }
            return fileName;
        }
        static string ChangeFileName(string oldName)
        {
            string extension = oldName.Substring(oldName.LastIndexOf("."));

            if (oldName.Length < 32)
            {
                oldName = oldName.Substring(0, oldName.LastIndexOf("."));
            }
            else
            {
                oldName = oldName.Substring(0,31);
            }
            
            return  Guid.NewGuid() + oldName + extension;
        }
         public static string CheckValidate(this IFormFile file, string type, int kb)
        {
            string result = "";
            if (!file.CheckSize(kb))
            {
                result += $"{file.FileName} faylinin hecmi {kb} kb-dan artiq olmamalidir";
            }
            if (!file.CheckType(type))
            {
                result += $" {file.FileName} faylinin tipi yalnishdir";
            }
            return result;
        }
        public static void DeleteFile(this string fileName, string root, string folder)
        {
            string path = Path.Combine(root, folder, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
