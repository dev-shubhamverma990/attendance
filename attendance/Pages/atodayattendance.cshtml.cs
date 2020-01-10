using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using attendance.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace attendance
{
    public class atodayattendanceModel : PageModel
    {
        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;

        public atodayattendanceModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
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
        public string employeesearch { get; set; }

        public IList<Employee> EmployeeData { get; private set; }
        List<Employee> EmployeeData1 = new List<Employee>();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        public async Task<IActionResult> OnGetAsync()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                connection.Open();

                string sql = "select name,timein,timeout from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,1,5)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',1,5) order by name";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                NpgsqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Employee p = new Employee();
                    p.Employeeid = Convert.ToString(dr["name"].ToString());
                    p.Timein = Convert.ToString(dr["timein"].ToString());
                    p.Timeout = Convert.ToString(dr["timeout"].ToString());
                    EmployeeData1.Add(p);
                }
                EmployeeData = EmployeeData1;
                dr.Close();

                connection.Close();

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            //try
            //{
            //    if (savemore == "searchstudent")
            //    {

            //        StudentData = (from a in _context.TblStudentRegistration
            //                       where a.DisplayName == studentsearch
            //                       select new TblStudentRegistration { StudentRegistrationId = a.StudentRegistrationId, Email = a.Email, DisplayName = a.DisplayName, Gender = a.Gender, ClassName = a.ClassName, Username = a.Username, Status = a.Status }).OrderBy(x => x.DisplayName).Distinct().ToList();
            //        if (StudentData.Count == 0)
            //        {
            //            ViewData["message"] = "No Data Found";
            //        }

            //    }


            //    else
            //    {
            //        StudentData = (from a in _context.TblStudentRegistration
            //                       where a.SchoolName == schoolsearch
            //                       select new TblStudentRegistration { StudentRegistrationId = a.StudentRegistrationId, Email = a.Email, DisplayName = a.DisplayName, Gender = a.Gender, ClassName = a.ClassName, Username = a.Username, Status = a.Status }).OrderBy(x => x.DisplayName).Distinct().ToList();
            //        if (StudentData.Count == 0)
            //        {
            //            ViewData["message"] = "No Data Found";
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return Page();
        }
    }
}