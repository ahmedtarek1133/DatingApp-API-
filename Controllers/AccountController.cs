using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is Taken");

        // using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();

        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        // user.PasswordSalt = hmac.Key;

        // context.Users.Add(user);
        // await context.SaveChangesAsync();

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

        // var user = new AppUser
        // {
        //     UserName  = registerDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     PasswordSalt = hmac.Key
        // };
        // context.Users.Add(user);
        // await context.SaveChangesAsync();

        // return new UserDto {
        //     Username = user.UserName ,
        //     Token = tokenService.CreateToken(user)
        // };

    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        // var user = await context.Users.FirstOrDefaultAsync(x => x.UserName .ToLower() == loginDto.Username.ToLower());
        var user = await userManager.Users
            .Include(p => p.Photos)
                .FirstOrDefaultAsync(x =>
        //             x.UserName == loginDto.Username.ToLower());
        // if (user == null) return Unauthorized("Invalid Username");

        x.NormalizedUserName == loginDto.Username.ToUpper());

        // using var hmac = new HMACSHA512(user.PasswordSalt);

        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        // }

        if (user == null || user.UserName == null) return Unauthorized("Invalid username");

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            // Token = tokenService.CreateToken(user)
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };

    }

    private async Task<bool> UserExists(string username)
    {

        // return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());

         return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper()); // Bob != bob

    }

}
