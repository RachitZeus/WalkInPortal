using BackendAPI.DTOS;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly walkin_portalContext _context;
        private const string SecretKey = "sRwvYz$LtzB#WqEf!aTdDgHkMnOpQrSt"; // Replace with your actual secret key
        private readonly SymmetricSecurityKey _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        public UserController(IUserService userService, walkin_portalContext context)
        {
            _userService = userService;
            _context = context;
        }
        [HttpGet]
        [HttpGet]
       
        [Route("/jobs")]
        public async Task<IActionResult> GetAllJobsAsync()
        {
            var jobDtoList = await _userService.GetAllJobsAsync();
            Console.Write(jobDtoList);
            return Ok(jobDtoList);
        }

        [HttpGet]
        [Route("/job/{id}")]
        public async Task<IActionResult> GetJobByIdAsync([FromRoute] int id)
        {
            var job = await _userService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        [Route("/apply")]
        public async Task<IActionResult> InsertApplicationAsync([FromBody] ApplicationRequest application)
        {
            Int32 applicationId = await _userService.InsertApplicationAsync(application);
            return Ok(applicationId);
        }
        [HttpPost]
        
        [Route("/login")]
        public async Task<IActionResult>LoginAsync(LoginRequest loginRequest)
        {
            Console.WriteLine(loginRequest);
            var user = await _userService.AuthenticateUser(loginRequest.username, loginRequest.password,loginRequest.rememberMe);
            Console.WriteLine(user);
            if (user == null)
            {
                // Unauthorized: Invalid username or password
                return Unauthorized();
            }

            // If authentication is successful, return a JWT token
            var token = GenerateJwtToken(user.username);
            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(string username)
        {
             // Convert userId to string
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(10), // Token expiration time
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet]
        [Route("/getapplication/{applicationId}")]
        public async Task<IActionResult> GetApplicationByIdAsync([FromRoute] int applicationId)
        {
            var application = _userService.GetApplicationByIdAsync(applicationId);
            return Ok(application);
        }

        [HttpPost]
        [Route("/user")]
        public async Task<IActionResult> RegisterUser(UserRegistrationRequest userRegistrationRequest)
        {


            await _userService.RegisterUser(userRegistrationRequest);

            return Ok();
        }
        [HttpGet]
        [Route("/getregistrationdata")]
        public async Task<IActionResult> getRegistrationDataAsync()
        {
            var collegesTask = await _context.Colleges.ToListAsync();
            var streamsTask = await _context.Streams.ToListAsync();
            var locationsTask = await _context.Locations.ToListAsync();
            var techsTask = await _context.Techs.ToListAsync();
            var qualificationsTask = await _context.Qualifications.ToListAsync();
            var rolesTask = await _context.Roles.ToListAsync();
            var applicationTypesTask = await _context.ApplicationTypes.ToListAsync();


            var registrationData = new RegistrationData
            {
                college = collegesTask,
                location = locationsTask,
                stream = streamsTask,
                qualification = qualificationsTask,
                tech = techsTask,
                role = rolesTask,
                applicationTypes = applicationTypesTask
            };
            return Ok(registrationData);
        }
    }
}
