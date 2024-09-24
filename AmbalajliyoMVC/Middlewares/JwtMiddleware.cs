using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmbalajliyoMVC.Middlewares
{
   public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Session.GetString("JWToken");

        if (!string.IsNullOrEmpty(token))
        {
            AttachUserToContext(context, token);
        }

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtTokenSettings:Key"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims.ToList();

            var userId = claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            // Kullanıcı kimliğini ayarla
            var claimsIdentity = new ClaimsIdentity(claims, "Bearer");
            context.User = new ClaimsPrincipal(claimsIdentity);

            // Role bilgisi ile oturum ayarları yapabilirsiniz
            context.Session.SetString("Role", role);
            context.Session.SetString("UserId", userId);
        }
        catch
        {
            // Token doğrulama başarısız olduysa, herhangi bir işlem yapmadan geçiyoruz
        }
    }
}

}
