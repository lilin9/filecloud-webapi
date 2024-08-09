using Microsoft.AspNetCore.Http;

namespace Domain.ViewModel {
    public class UserAvatarVm {
        public IFormFile AvatarFile { get; set; }

        public string UserId { get; set; }
    }
}
