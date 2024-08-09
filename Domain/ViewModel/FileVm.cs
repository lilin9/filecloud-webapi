using System.Text.Json.Serialization;
namespace Domain.Vo {
    public class FileVo {
        [JsonIgnore]
        public Guid GuidFileId { get; set; } = Guid.Empty;
        public string FileId { get; set; }
        public string FileName { get; set; }
    }
}
