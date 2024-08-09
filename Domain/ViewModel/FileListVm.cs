using System.Text.Json.Serialization;

namespace Domain.Vo {
    public class FileListVm {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? ParentId { get; set; }
        [JsonIgnore]
        public Guid? GuidParentId { get; set; }
    }
}
