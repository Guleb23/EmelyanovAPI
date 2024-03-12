using Dapper;
using EmelyanovDiplom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EmelyanovDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UslugiController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UslugiController(IConfiguration configuration)
        {
            this.configuration = configuration;

        }


        [HttpGet("/GetAllUslugi")]
        public async Task<ActionResult<IEnumerable<Uslugi>>> GetAllUslugi()
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            IEnumerable<Uslugi>? uslugi = await connection.QueryAsync<Uslugi>("Select * from Uslugi");
            return Ok(uslugi);
        }


        [HttpGet("/GetUslugiById/{providerId}")]
        public async Task<ActionResult<IEnumerable<Uslugi>>> GetUslugiById(int providerId)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            IEnumerable<Uslugi>? uslugi = await connection.QueryAsync<Uslugi>("Select * from Uslugi where Provider =@Provider", new {Provider = providerId});
            return Ok(uslugi);
        }



        [HttpPost("/CreateUslugu")]
        public async Task<ActionResult<Uslugi>> CreateUslugu(Uslugi usluga)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into Uslugi (  Name,  Description,  Price,  Provider, Category) values (  @Name,  @Description,  @Price,  @Provider, @Category)", usluga);
            return Ok(usluga);
        }
    }
}
