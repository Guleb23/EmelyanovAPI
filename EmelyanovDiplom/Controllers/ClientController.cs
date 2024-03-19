using Dapper;
using EmelyanovDiplom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace EmelyanovDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ClientController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        [HttpPost("/CreateClient")]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));


            await connection.ExecuteAsync("insert into Client ( FirstName, LastName, Phone, Login, Password, Favorite) values (@FirstName, @LastName, @Phone, @Login, @Password, @Favorite)", client);


            return Ok(client);
        }
        [HttpGet("/GetAllClients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var clients = await connection.QueryAsync<Client>("select * from Client");
            return Ok(clients);
        }
        [HttpGet("/GetAllClientsById/{clientId}")]
        public async Task<ActionResult<Client>> GetAllClientsById(int clientId)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var clients = await connection.QueryAsync<Client>("select * from Client where Id=@Id", new { Id = clientId});
            return Ok(clients);
        }
        [HttpGet("LoginClient/{userPhone}/{userPassword}")]
        public async Task<ActionResult<Client>> LoginUser(string userPhone, string userPassword)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            var user = await connection.QueryFirstOrDefaultAsync<Client>("Select * from Client where Login=@Login and Password=@Password ", new { Login = userPhone, Password = userPassword });
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);


        }
        [HttpPut("/UpdateClient")]
        public async Task<ActionResult<Client>> UpdateClient(Client user)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Client set  FirstName = @FirstName, LastName = @LastName, Phone = @Phone, Login = @Login, Password  = @Password where Id = @id",
                user);
            return Ok();
        }
    }
}
