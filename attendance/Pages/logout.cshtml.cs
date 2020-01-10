using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace attendance
{
    public class logoutModel : PageModel
    {
        private readonly IConfiguration _configuration;
        HttpClient _client = new HttpClient();

        public logoutModel(IConfiguration config)
        {
            _configuration = config;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_configuration["base_Url"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> OnGet()
        {
            
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
          //  Response.Redirect("http://103.118.157.29:8083/Login");
            return Page();

        }
    }
}