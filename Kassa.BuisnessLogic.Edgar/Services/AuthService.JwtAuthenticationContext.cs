using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal partial class AuthService
{
    private class JwtAuthenticationContext : IAuthenticationContext
    {
        public static readonly JwtAuthenticationContext NotAuthenticated = new()
        {
            User = null,
            Member = null
        };

        public UserDto? User
        {
            get; set;
        }

        public MemberDto? Member
        {
            get; set;
        }

        public string Token
        {
            get; set;
        } = string.Empty;

        public bool IsAuthenticated => User is not null || Member is not null || !string.IsNullOrWhiteSpace(Token);
    }
}
