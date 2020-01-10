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
    public class monthlyreportModel : PageModel
    {
        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;

        public monthlyreportModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
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
        public string monthsearch { get; set; }

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

        public IList<Employee> AttendanceData { get; private set; }
        List<Employee> AttendanceData1 = new List<Employee>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                string month = indianTime.ToString("MM"); 
                int year = indianTime.Year;
                string txtdate = "00-" + month + "-" + year;
                using (NpgsqlConnection connection = new NpgsqlConnection())
                {
                    connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                    connection.Open();

                    string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + HttpContext.Session.GetString("Username") + "' order by date";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    NpgsqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Employee p = new Employee();
                        p.Employeeid = Convert.ToString(dr["name"].ToString());
                        p.Timein = Convert.ToString(dr["timein"].ToString());
                        p.Timeout = Convert.ToString(dr["timeout"].ToString());
                        string date1 = Convert.ToString(dr["date"].ToString());
                        DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                        p.Date = tmpDate.ToString("dd-MM-yyyy");
                        AttendanceData1.Add(p);
                    }
                    AttendanceData = AttendanceData1;
                    dr.Close();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            try
            {
                if(savemore== "monthlysearch")
                {
                    int month = Convert.ToInt32(monthsearch);
                    int year = indianTime.Year;
                    string txtdate = "00-" + month + "-" + year;
                    using (NpgsqlConnection connection = new NpgsqlConnection())
                    {
                        connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                        connection.Open();

                        string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + HttpContext.Session.GetString("Username") + "' order by date";
                        NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                        NpgsqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            Employee p = new Employee();
                            p.Employeeid = Convert.ToString(dr["name"].ToString());
                            p.Timein = Convert.ToString(dr["timein"].ToString());
                            p.Timeout = Convert.ToString(dr["timeout"].ToString());
                            string date1 = Convert.ToString(dr["date"].ToString());
                            DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                            p.Date = tmpDate.ToString("dd-MM-yyyy");
                            AttendanceData1.Add(p);
                        }
                        AttendanceData = AttendanceData1;

                        dr.Close();

                        connection.Close();
                    }
                    ModelState.Clear();
                    return Page();
                }
                else
                {
                    if (monthsearch == "0")
                    {
                        string month = indianTime.ToString("MM");
                        int year = indianTime.Year;
                        string txtdate = "00-" + month + "-" + year;
                        using (NpgsqlConnection connection = new NpgsqlConnection())
                        {
                            connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                            connection.Open();

                            string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + HttpContext.Session.GetString("Username") + "' order by date";
                            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                            NpgsqlDataReader dr;
                            dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Employee p = new Employee();
                                p.Employeeid = Convert.ToString(dr["name"].ToString());
                                p.Timein = Convert.ToString(dr["timein"].ToString());
                                p.Timeout = Convert.ToString(dr["timeout"].ToString());
                                string date1 = Convert.ToString(dr["date"].ToString());
                                DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                                p.Date = tmpDate.ToString("dd-MM-yyyy");
                                AttendanceData1.Add(p);
                            }
                            AttendanceData = AttendanceData1;
                            dr.Close();

                            connection.Close();
                        }
                    }
                    else
                    {
                        int month = Convert.ToInt32(monthsearch);
                        int year = indianTime.Year;
                        string txtdate = "00-" + month + "-" + year;
                        using (NpgsqlConnection connection = new NpgsqlConnection())
                        {
                            connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                            connection.Open();

                            string sql = "select name,timein,timeout,date from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + txtdate + "',3,8) and employeeid='" + HttpContext.Session.GetString("Username") + "' order by date";
                            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                            NpgsqlDataReader dr;
                            dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                Employee p = new Employee();
                                p.Employeeid = Convert.ToString(dr["name"].ToString());
                                p.Timein = Convert.ToString(dr["timein"].ToString());
                                p.Timeout = Convert.ToString(dr["timeout"].ToString());
                                string date1 = Convert.ToString(dr["date"].ToString());
                                DateTime tmpDate = DateTime.ParseExact(date1, "dd-MM-yyyy HH:mm", null);
                                p.Date = tmpDate.ToString("dd-MM-yyyy");
                                AttendanceData1.Add(p);
                            }
                            AttendanceData = AttendanceData1;

                            dr.Close();

                            connection.Close();
                        }
                    }


                    string sWebRootFolder = _hostingEnvironment.WebRootPath;
                    string sFileName = @"'" + HttpContext.Session.GetString("DisplayName") + "'_MonthlyAttendance.xlsx";
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
                        row.CreateCell(1).SetCellValue("Date");
                        row.CreateCell(2).SetCellValue("Time In");
                        row.CreateCell(3).SetCellValue("Time Out");

                        foreach (var segment in AttendanceData)
                        {
                            row = excelSheet.CreateRow(i);
                            row.CreateCell(0).SetCellValue(i);
                            row.CreateCell(1).SetCellValue(segment.Date);
                            row.CreateCell(2).SetCellValue(segment.Timein);
                            row.CreateCell(3).SetCellValue(segment.Timeout);

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
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}