namespace ReportService.Requests
{

    public class UserImageUploadRequest
    {
        public string UserId { get; set; }
        public string ImageBase64String { get; set; }
    }
}
