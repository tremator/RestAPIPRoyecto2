using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoWeb2.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using ProyectoWeb2Api.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace ProyectoWeb2.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class UsersController: ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly SMSSettings twilioSettings;

       public UsersController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            twilioSettings = new SMSSettings();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id){
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }
            user.role = await _context.Roles.FindAsync(user.roleId);
            return user;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Getusers(){
            return await _context.Users.ToArrayAsync();
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserInfo info){
            var results = from users in _context.Users select users;
            var user = await results.Where((user) => user.email == info.email && user.password == info.password)?.SingleAsync();

            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.email));

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(4),
               SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            string token =  tokenHandler.WriteToken(createdToken);

            user.token = token;
            user.isLogged = false;
            if(user.registerConfirmation == true){
                user.authCode = SendSMSMessage(user.phone);
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return user;

        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(User user){
            user.registerConfirmation = false;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var userCreated = CreatedAtAction("GetUser", new{id = user.id}, user);
            sendEmail(user);
            return userCreated;
            
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(long id, User user){
            if (id != user.id) {
                return BadRequest();
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(long id){
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public  void sendEmail(User user){
            using (MailMessage message = new MailMessage("dmsolisa@gmail.com",user.email))
            {
                message.Subject = "EmailConfirmation";
                message.Body = "https://localhost:5001/api/users/confirmation/"+user.id;
                message.IsBodyHtml = false;

                using(SmtpClient smtp = new SmtpClient()){
                    NetworkCredential credentials = new NetworkCredential("dmsolisa@gmail.com","dmsolisa");
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = credentials;
                    smtp.Port = 587;
                    smtp.Send(message);
                }
            }
        }
        [HttpGet("confirmation/{id}")]
        public void emailConfirmation(long id){
            var user = _context.Users.Find(id);
            user.registerConfirmation = true;
             _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public string SendSMSMessage(string phone){
            var random = new Random();
            var code = random.Next(1000,9999);
            TwilioClient.Init(
                twilioSettings.TwilioAccountID,
                twilioSettings.TwilioAuthToken
            );

            var message = MessageResource.Create(
                from: new PhoneNumber(twilioSettings.TwilioPhoneNumber),
                to: new PhoneNumber("+506"+phone),
                body: "this is your login code: "+ code
            );
            
            return code.ToString();
        }
        [HttpPost("verifyCode")]
        public User verifyCode(CodeInfo info){
            var user = _context.Users.Find(info.userId);
            if(user.authCode == info.code){
                user.isLogged = true;
            }
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            return user;
        }

        [HttpPost("logout/{id}")]
        public void logout(long id){
            var user = _context.Users.Find(id);
            user.isLogged = false;
            user.token = "";
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            
        }
    }
}