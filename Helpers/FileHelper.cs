namespace ReportService.Helpers
{
    public class FileHelper
    {
        public bool IsFileCompliant(byte[] byteImage, ref string sMessage)
        {
            try
            {
                // Upload the file if less than 1MB (1048576)  -------- 2 MB (2097152)
                if (byteImage.Length <= 1048576)
                {
                    return true;
                }
                sMessage = "File too large.  Image must be less than 1MB in size.";
                return false;
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
                return false;
            }
        }
    }
}
