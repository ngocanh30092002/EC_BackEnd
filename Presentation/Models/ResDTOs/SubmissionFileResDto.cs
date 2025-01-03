﻿namespace EnglishCenter.Presentation.Models.ResDTOs
{
    public class SubmissionFileResDto
    {
        public long SubmissionFileId { set; get; }
        public string? FilePath { set; get; }
        public string? LinkUrl { set; get; }
        public string? Status { set; get; }
        public string? UploadAt { set; get; }
        public string? UploadBy { set; get; }
        public long? EnrollId { set; get; }
        public string? FileSize { set; get; }
        public string UserImage { set; get; }
        public string UserName { set; get; }
    }
}
