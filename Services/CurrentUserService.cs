using System.Security.Claims;

namespace friendzone_backend.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (value == null) throw new UnauthorizedAccessException();
                return Guid.Parse(value);
            }
        }

        public string Username
        {
            get
            {
                return _httpContextAccessor.HttpContext?
                    .User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            }
        }
    }
}