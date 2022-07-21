using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using API.Data;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
            
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("UserName already taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
        
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string Username)
        {
          return await _context.Users.AnyAsync(x=>x.UserName == Username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
            .SingleOrDefaultAsync(x=>x.UserName == loginDto.Username);
            if(user == null) return Unauthorized("Invalid Username");
            
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return  new UserDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            
        }
    }
}