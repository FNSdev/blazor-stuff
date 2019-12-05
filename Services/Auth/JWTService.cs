using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;  
using Microsoft.IdentityModel.Tokens;  
using System;  
using System.IdentityModel.Tokens.Jwt;  
using System.Security.Claims;  
using System.Text;  
using System.Threading.Tasks;
using System.Collections.Generic;

using hephaestus.Models;


public interface IJWTService
{
    Task<string> GenerateJSONWebToken(User user);
    Task<List<Claim>> GetClaims(User user);
}


public class JWTService : IJWTService
{
    private IConfiguration _config;
    private UserManager<User> _userManager;
    private List<Claim> _claims = null;
    public JWTService(IConfiguration config, UserManager<User> userManager)
    {
        _config = config;
        _userManager = userManager;
    }

    public async Task<List<Claim>> GetClaims(User user)
    {
        if(_claims is null)
        {
            _claims = new List<Claim>() {  
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };  

            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                _claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        return _claims;
    }

    public async Task<string> GenerateJSONWebToken(User user)  
    {  
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));  
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);  

        var token = new JwtSecurityToken(
            _config["JWT:Issuer"],  
            _config["JWT:Issuer"],  
            await GetClaims(user),  
            expires: DateTime.Now.AddDays(365),  
            signingCredentials: credentials);  

        return new JwtSecurityTokenHandler().WriteToken(token);  
    }  
}
