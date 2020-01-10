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

namespace attendance
{
    public class loginModel : PageModel
    {
        private readonly attendanceContext _context;
        private readonly IConfiguration _configuration;
        HttpClient _client;

        public loginModel(IConfiguration config, attendanceContext context)
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
        public string Password { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            try
            {

                var user = (from a in _context.Login where a.Username == Username select a.Username).FirstOrDefault();
                if (user == null)
                {
                    await Response.WriteAsync("<script language='javascript'>window.alert('UserName is invalid');window.location='login';</script>");
                    return Page();
                }
                else
                {

                }

                var pass = (from a in _context.Login
                            where a.Username == Username && a.Password == Encrypt(Password)
                            select new { a.Password, a.Username,a.Name }).FirstOrDefault();

                if (pass == null)
                {
                    await Response.WriteAsync("<script language='javascript'>window.alert('Password is invalid');window.location='Login';</script>");
                    return Page();
                }
                else
                {
                    if (pass.Username.ToString() == "admin")
                    {
                        HttpContext.Session.SetString("DisplayName", "Admin");
                        HttpContext.Session.SetString("password", pass.Password.ToString());
                        return RedirectToPage("/adashboard");
                    }
                    else
                    {

                        HttpContext.Session.SetString("DisplayName", pass.Name.ToString());
                        HttpContext.Session.SetString("password", pass.Password.ToString());
                        HttpContext.Session.SetString("Username", pass.Username.ToString());
                        return RedirectToPage("/Dashboard");
                    }

                }

            }
            catch (Exception ex)
            {
                await Response.WriteAsync("<script language='javascript'>window.alert('Something Went Wrong.');window.location='Login';</script>");
                throw ex;
            }
            return Page();
        }

        public string Encrypt(string value)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}