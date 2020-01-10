using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using attendance.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace attendance
{
    public class changepasswordModel : PageModel
    {
        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        string layoutImagePath = String.Empty;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;

        public changepasswordModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = config;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_configuration["base_Url"]);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _connection = _configuration["ConnectionStrings:Princetonhive"];
            _context = context;
        }

        [BindProperty]
        public string currentpassword { get; set; }
        [BindProperty]
        public string newpassword { get; set; }
        [BindProperty]
        public string confirmpassword { get; set; }

        public void OnGet()
        {
           
            
        }

        public string Encrypt(string value)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Decrypt(string value)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            try
            {

                var princetonhive = new attendanceContext();
                
                var passwordupdate = princetonhive.Login.Where(x => x.Username == (HttpContext.Session.GetString("Username"))).SingleOrDefault();
                if (passwordupdate.Password == Encrypt(currentpassword))
                 {
                    passwordupdate.Password = Encrypt(newpassword);
                    princetonhive.SaveChanges();
                    await Response.WriteAsync("<script language='javascript'>window.alert('Password updated Successfully');window.location='logout';</script>");
                }
                else
                {
                    await Response.WriteAsync("<script language='javascript'>window.alert('Current password not match.. ');window.location='changepassword';</script>");
                }
                
                 
            }

            catch (Exception ex)
            {

                await Response.WriteAsync("<script language='javascript'>window.alert('Something Went Wrong.');window.location='changepassword';</script>");
                throw ex;
            }


            return Page();
        }
    }
}