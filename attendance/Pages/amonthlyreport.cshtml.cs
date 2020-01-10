using System;
using System.Collections.Generic;
using System.IO;
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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace attendance
{
    public class amonthlyreportModel : PageModel
    {
        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;

        public amonthlyreportModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
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
        public string monthname { get; set; }

        public class Employeedata1
        {
            public int Id { get; set; }
            public string Employeeid { get; set; }
            public string Timein { get; set; }
            public string Timeout { get; set; }
            public string Date { get; set; }
            public string Name { get; set; }
        }

        public IList<Employeedata1> EmployeeData { get; private set; }
        List<Employeedata1> EmployeeData1 = new List<Employeedata1>();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        public async Task<IActionResult> OnGetAsync(string username, string month)
        {
            HttpContext.Session.SetString("ausername", username);
            HttpContext.Session.SetString("amonth", month);
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                connection.Open();
                int year = indianTime.Year;
                if(month=="01")
                {
                    monthname = "January";
                }
                else if (month == "02")
                {
                    monthname = "February";
                }
                else if(month == "03")
                {
                    monthname = "March";
                }
                else if(month == "04")
                {
                    monthname = "April";
                }
                else if(month == "05")
                {
                    monthname = "May";
                }
                else if(month == "06")
                {
                    monthname = "June";
                }
                else if(month == "07")
                {
                    monthname = "July";
                }
                else if (month == "08")
                {
                    monthname = "August";
                }
                else if (month == "09")
                {
                    monthname = "September";
                }
                else if (month == "10")
                {
                    monthname = "October";
                }
                else if (month == "11")
                {
                    monthname = "November";
                }
                else if (month == "12")
                {
                    monthname = "December";
                }
                string txtdate = "00-" + month + "-" + year;
                string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + username + "' order by date";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                NpgsqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Employeedata1 p = new Employeedata1();
                    p.Employeeid = Convert.ToString(dr["name"].ToString());
                    p.Timein = Convert.ToString(dr["timein"].ToString());
                    p.Timeout = Convert.ToString(dr["timeout"].ToString());
                    string date1 = Convert.ToString(dr["date"].ToString());
                    DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                    p.Date = tmpDate.ToString("dd-MM-yyyy");
                   // p.Date = date1.ToString("dd-MM-yyyy");
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
            try
            {

                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                    connection.Open();
                    int year = indianTime.Year;
                    int month = Convert.ToInt32(HttpContext.Session.GetString("amonth"));
                    string txtdate = "00-" + month + "-" + year;
                    string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + HttpContext.Session.GetString("ausername") + "' order by date";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Employeedata1 p = new Employeedata1();
                        p.Name = Convert.ToString(dr["name"].ToString());
                        p.Timein = Convert.ToString(dr["timein"].ToString());
                        p.Timeout = Convert.ToString(dr["timeout"].ToString());
                        string date1 = Convert.ToString(dr["date"].ToString());
                        DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                        p.Date = tmpDate.ToString("dd-MM-yyyy");
                        EmployeeData1.Add(p);
                    }
                    EmployeeData = EmployeeData1;
                    dr.Close();

                    connection.Close();

                }


                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                    string sFileName = @"MonthlyAttendance.xlsx";
                    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                    var memory = new MemoryStream();
                    using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                    {

                        IWorkbook workbook;
                        workbook = new XSSFWorkbook();
                        ISheet excelSheet = workbook.CreateSheet("Employee");
                        IRow row = excelSheet.CreateRow(0);
                        int i = 1;
                        row.CreateCell(0).SetCellValue("S.No");
                        row.CreateCell(1).SetCellValue("Name");
                        row.CreateCell(2).SetCellValue("Date");
                        row.CreateCell(3).SetCellValue("Time In");
                        row.CreateCell(4).SetCellValue("Time Out");

                        foreach (var segment in EmployeeData)
                        {
                            row = excelSheet.CreateRow(i);
                            row.CreateCell(0).SetCellValue(i);
                            row.CreateCell(1).SetCellValue(segment.Name);
                            row.CreateCell(2).SetCellValue(segment.Date);
                            row.CreateCell(3).SetCellValue(segment.Timein);
                            row.CreateCell(4).SetCellValue(segment.Timeout);

                            i++;
                        }


                        workbook.Write(fs);
                    }
                    using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Page();
        }
    }
}