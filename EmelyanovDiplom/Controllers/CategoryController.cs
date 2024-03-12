using Dapper;
using EmelyanovApp.Models;
using EmelyanovDiplom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmelyanovDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     public class CategoryController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public CategoryController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }



        [HttpGet("/GetAllCategory")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategory()
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var categories = await connection.QueryAsync<Category>("select * from Category");
            return Ok(categories);
        }


        [HttpGet("/GetCategoryById/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryById(int categoryId)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            IEnumerable<Category>? category = await connection.QueryAsync<Category>("Select * from Category where Id = @Id", new { Id = categoryId });
            return Ok(category);
        }
    }
}
