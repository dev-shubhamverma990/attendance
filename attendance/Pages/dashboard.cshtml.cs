using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using attendance.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace attendance
{
    public class dashboardModel : PageModel
    {
        private readonly attendanceContext _context;
        private readonly IConfiguration _configuration;
        HttpClient _client;

        public dashboardModel(IConfiguration config, attendanceContext context)
        {
            _configuration = config;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_configuration["base_Url"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public int TotalPresent { get; set; }
        [BindProperty]
        public int Totalabsent { get; set; }
        [BindProperty]
        public int TotalWFH { get; set; }

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username");

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                connection.Open();

                string sql = "select count(id) as id from employee where timein IS NOT NULL and SUBSTRING(date,3,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',3,5) and employeeid='" + Username + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                NpgsqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {  
                    TotalPresent = Convert.ToInt32(dr["id"].ToString());   
                }
               
                dr.Close();

                string sql1 = "select count(id) as id from employee where timein IS NULL and SUBSTRING(date,3,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',3,5) and employeeid='" + Username + "'";
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connection);
                NpgsqlDataReader dr1;
                dr1 = cmd1.ExecuteReader();
                while (dr1.Read())
                {
                    Totalabsent = Convert.ToInt32(dr1["id"].ToString());
                }

                dr1.Close();
                connection.Close();

            }
        }
    }
}