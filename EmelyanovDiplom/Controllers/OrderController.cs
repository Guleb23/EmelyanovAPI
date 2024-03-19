using Dapper;
using EmelyanovDiplom.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SixLabors.ImageSharp;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using VetCV.HtmlRendererCore.PdfSharpCore;
using System.Text.Json.Serialization;
using System.Collections;
using System.Linq;

namespace EmelyanovDiplom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration configuration;
        static HttpClient client;

        public OrderController(IConfiguration configuration)
        {
            this.configuration = configuration;
            client = new HttpClient();
        }





        [HttpPost("/CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));


            await connection.ExecuteAsync("insert into [Order] (IdClient, IdUslugi, DateOformleniya) values (@IdClient, @IdUslugi, @DateOformleniya)", order);
            return Ok(order);
        }


        [HttpGet("/GetAllOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var orders = await connection.QueryAsync<Order>("select * from [Order]");
            return Ok(orders);
        }

        [HttpGet("/GetPDF/{userId}/{uslugId}")]
        public async Task<ActionResult> GetPDF(int userId, int uslugId)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var orders = await connection.QueryFirstAsync<Order>("select * from [Order] where IdClient=@IdClient and IdUslugi=@IdUslugi", new { IdClient = userId, IdUslugi = uslugId });
            var userFoder = await GetClient(userId);
            var curUslg = await GetUsluga(orders.IdUslugi);
            var curProvider = await GetProvider(curUslg.Provider);
            PdfDocument pdf = new PdfDocument();
            string html = $"<p style=\"text-align: center;\"><strong>АКТ ОКАЗАНИИ УСЛУГИ {curUslg.Name}</strong>" +
                $"</p>\r\n<p style=\"text-align: center;\"><strong>&nbsp;</strong></p>\r\n<table style=\"height: 11px; width: 753.5px;\" border=\"0\">\r\n<tbody>\r\n<tr style=\"height: 13px;\">" +
                $"\r\n<td style=\"width: 368px; height: 13px;\">г.Москва</td>\r\n<td style=\"width: 368.5px; text-align: right; height: 13px;\">{orders.DateOformleniya}</td>\r\n</tr>\r\n</tbody>\r\n</table>" +
                $"\r\n<p>&nbsp;</p>\r\n<table style=\"width: 752px;\">\r\n<tbody>\r\n<tr>\r\n<td style=\"width: 378.453px; text-align: left;\">Информация о клиенте:</td>\r\n" +
                $"<td style=\"width: 375.547px; text-align: center;\">\r\n<p style=\"text-align: left;\">Информация о поставщике:</p>\r\n<p>&nbsp;</p>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n" +
                $"<table style=\"width: 753px;\">\r\n<tbody>\r\n<tr>\r\n<td style=\"width: 568.312px;\">\r\n<p>ФИО:{userFoder.FirstName + " "} {userFoder.LastName}</p>\r\n<p>Номер телефона: {userFoder.Phone}</p>" +
                $"\r\n</td>\r\n<td style=\"width: 548.688px;\">\r\n<p>Название: {curProvider.Name}</p>\r\n<p>Номер телефона:{curProvider.Password}</p>\r\n<p>ИНН:{curProvider.INN}</p>\r\n</td>\r\n</tr>\r\n</tbody>" +
                $"\r\n</table>\r\n<p style=\"text-align: center;\">Информация об оказании услуги</p>\r\n<p style=\"text-align: left;\">Цена:{curUslg.Price}</p>" +
                $"\r\n<p style=\"text-align: left;\">Описание:{curUslg.Description}</p>\r\n<p style=\"text-align: center;\">&nbsp;Обязанности сторон:</p>\r\n" +
                $"<p style=\"text-align: left;\">1.Исполнитель обязан выполнить всю, описанную работу</p>" +
                $"\r\n<p style=\"text-align: left;\">2.Исполнитель не может менять цену услуги после оформления, акуальная цена находиться в данном договоре</p>\r\n" +
                $"<p style=\"text-align: left;\">3.Заказчик должен оплатить полную стоимость услуги</p>" +
                $"\r\n<p style=\"text-align: left;\">&nbsp;</p>\r\n<p style=\"text-align: left;\">&nbsp;</p>\r\n<p style=\"text-align: left;\">&nbsp;</p>\r\n<hr />\r\n" +
                $"<p style=\"text-align: left;\">1. В случае возникания споров исполбзуйте данный документ</p>";
            VetCV.HtmlRendererCore.PdfSharpCore.PdfGenerator.AddPdfPages(pdf, html, PageSize.A4);

            byte[]? resp;
            using (MemoryStream stream = new MemoryStream())
            {
                pdf.Save(stream);
                resp = stream.ToArray();
            }
            string filename = "asdasd" + "qweq" + ".pdf";
            return File(resp, "application/pdf");


        }


        private async Task<Client> GetClient(int id)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var resp = await connection.QueryFirstAsync<Client>("select * from Client where Id=@Id", new { Id = id });
            return resp;
        }

        private async Task<Uslugi> GetUsluga(int id)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var resp = await connection.QueryFirstAsync<Uslugi>("select * from Uslugi where Id=@Id", new { Id = id });
            return resp;
        }

        private async Task<Provider> GetProvider(int id)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var resp = await connection.QueryFirstAsync<Provider>("select * from Provider where Id=@Id", new { Id = id });
            return resp;
        }


        [HttpGet("GetCountOrders/{userId}")]
        public async Task<int> GetCountOrders(int userId)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            var resp = await connection.QueryAsync<Order>("select * from [Order] where IdClient=@IdClient", new { IdClient = userId });
            return resp.Count();
        }
    }
}