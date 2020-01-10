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
using Npgsql;

namespace attendance
{
    public class attendanceModel : PageModel
    {

        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        string layoutImagePath = String.Empty;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;
        string count;
        string EId;

        public attendanceModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
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
        public string timein { get; set; }
        [BindProperty]
        public string timeout { get; set; }


        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        public void OnGet()
        {
            timein = indianTime.ToString("HH:mm");
            timeout = indianTime.ToString("HH:mm");
        }

        //public void GetUser_IP()
        //{
        //    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (string.IsNullOrEmpty(ipAddress))
        //    {
        //        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //}

        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            try
            {
                
                var princetonhive = new attendanceContext();
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                    
                    if (savemore == "Save")
                    {
                       
                        connection.Open();
                        string sql = "select count(*) as id from employee where timein IS NOT NULL and SUBSTRING(date,1,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',1,5) and employeeid='" + HttpContext.Session.GetString("Username") + "' and timein IS NOT null";
                        NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                        NpgsqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            count = Convert.ToString(dr["id"].ToString());   
                        }
                       
                        dr.Close();
  
                        if (count == "0")
                        {
                            var studentd = new Employee()
                            {
                                Employeeid = HttpContext.Session.GetString("Username"),
                                Timein = timein,
                                Date = indianTime.ToString("dd-MM-yyyy HH:mm")

                            };
                            princetonhive.Employee.Add(studentd);
                            princetonhive.SaveChanges();
                            await Response.WriteAsync("<script language='javascript'>window.alert('Data Saved Successfully');window.location='attendance';</script>");
                        }
                        else
                        {
                            await Response.WriteAsync("<script language='javascript'>window.alert('Attendance already exist');window.location='attendance';</script>");
                        }
                        connection.Close();
                    }
                    else
                    {
                        connection.Open();

                        string sql = "select count(*) as id from employee where SUBSTRING(date,1,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',1,5) and employeeid='" + HttpContext.Session.GetString("Username") + "' and timeout IS NOT null";
                        NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                        NpgsqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            count = Convert.ToString(dr["id"].ToString());
                        }

                        dr.Close();

                        if (count == "0")
                        {
                            string sql2 = "select id from employee where SUBSTRING(date,1,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',1,5) and employeeid='" + HttpContext.Session.GetString("Username") + "'";
                            NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connection);
                            NpgsqlDataReader dr2;
                            dr2 = cmd2.ExecuteReader();
                            while (dr.Read())
                            {
                                EId = Convert.ToString(dr2["id"].ToString());
                            }

                            dr.Close();

                            if (EId != null)
                            {
                                var timeoutupdate = princetonhive.Employee.Where(x => x.Id == Convert.ToInt32(EId)).SingleOrDefault();
                                if (timeoutupdate != null)
                                {

                                    timeoutupdate.Timeout = timeout;
                                    timeoutupdate.Dateout = indianTime.ToString("dd-MM-yyyy HH:mm");
                                    princetonhive.SaveChanges();
                                    await Response.WriteAsync("<script language='javascript'>window.alert('Data Saved Successfully');window.location='attendance';</script>");
                                }
                                else
                                {
                                    await Response.WriteAsync("<script language='javascript'>window.alert('Something went wrong');window.location='attendance';</script>");
                                }
                            }
                            
                        }
                        else
                        {
                            await Response.WriteAsync("<script language='javascript'>window.alert('Attendance already exist');window.location='attendance';</script>");
                        }
                    }


                }   
               
            }

            catch (Exception ex)
            {
             
                await Response.WriteAsync("<script language='javascript'>window.alert('Something Went Wrong.');window.location='attendance';</script>");
                throw ex;
            }


            return Page();
        }
    }
}