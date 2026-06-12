namespace api_gateway.Helpers
{
    public static class FileValidator
    {
        public static bool IsInvalidImage(IFormFile? file)
        {
            return file is null ||
                   file.Length == 0 ||
                   !file.ContentType.StartsWith("image/");
        }
    }
}