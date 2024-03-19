using Dapper;
using EmelyanovDiplom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EmelyanovDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ProviderController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }

        
        [HttpGet("/GetAllProviders")]
        public async Task<ActionResult<IEnumerable<Provider>>> GetAllProvider()
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var providers = await connection.QueryAsync<Provider>("Select * from Provider");
            return Ok(providers);
        }
        [HttpGet("LoginUser/{userPhone}/{userPassword}")]
        public async Task<ActionResult<Provider>> LoginUser(string userPhone, string userPassword)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            var user = await connection.QueryFirstOrDefaultAsync<Provider>("Select * from Provider where Login=@Login and Password=@Password ", new { Login = userPhone, Password = userPassword });
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);


        }
        [HttpPost("/CreateProvider")]
        public async Task<ActionResult<Provider>> CreateProvider(Provider provider)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));


            await connection.ExecuteAsync("insert into Provider (Name, INN, Login, Password) values (@Name, @INN, @Login, @Password)", provider);


            return Ok(provider);
        }
    }
}
