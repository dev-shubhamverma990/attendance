using System;
using System.Collections.Generic;
using System.IO;
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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace attendance
{
    public class amonthlyattendanceModel : PageModel
    {
        private readonly string _connection = string.Empty;
        private readonly IConfiguration _configuration;
        HttpClient _client;
        private IHostingEnvironment _hostingEnvironment;
        private readonly attendanceContext _context;

        public amonthlyattendanceModel(IConfiguration config, IHostingEnvironment hostingEnvironment, attendanceContext context)
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

        public IList<EmployeeAttendance> Attendancedata { get; private set; }
        List<EmployeeAttendance> Attendancedata1 = new List<EmployeeAttendance>();

        public class EmployeeAttendance
        {
            public int totalworkingdays { get; set; }
            public int totalpresent { get; set; }
            public int totalabsent { get; set; }
            public string name { get; set; }
            public string monthname { get; set; }
            public string username { get; set; }
        }

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        

        public async Task<IActionResult> OnGetAsync()
        {
            int month = Convert.ToInt32(indianTime.ToString("MM"));
            int year = indianTime.Year;
            var weeks = getWeekdatesandDates(month, year);
            int days = DateTime.DaysInMonth(year, month);
            int totalworkingdays = days - Convert.ToInt32(weeks.Count);
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                connection.Open();

                string sql = "select name,username,count(*) as id from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,8)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',3,8) group by name,username order by name";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                NpgsqlDataReader dr;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    EmployeeAttendance p = new EmployeeAttendance();
                    p.totalworkingdays = totalworkingdays;
                    p.totalpresent = Convert.ToInt32(dr["id"].ToString());
                    p.totalabsent = totalworkingdays- Convert.ToInt32(dr["id"].ToString());
                    p.name = Convert.ToString(dr["name"].ToString());
                    p.username = Convert.ToString(dr["username"].ToString());
                    p.monthname = Convert.ToString(indianTime.ToString("MM"));
                    Attendancedata1.Add(p);
                }
                Attendancedata = Attendancedata1;
                dr.Close();

                connection.Close();

            }

            return Page();
        }

        public List<DateTime> getWeekdatesandDates(int Month, int Year)
        {
            List<DateTime> weekdays = new List<DateTime>();

            DateTime firstOfMonth = new DateTime(Year, Month, 1);

            DateTime currentDay = firstOfMonth;
            while (firstOfMonth.Month == currentDay.Month)
            {
                DayOfWeek dayOfWeek = currentDay.DayOfWeek;
                if (dayOfWeek == DayOfWeek.Sunday)
                    weekdays.Add(currentDay);

                currentDay = currentDay.AddDays(1);
            }

            return weekdays;
        }

        public async Task<IActionResult> OnPostAsync(string savemore)
        {
            try
            {
                if (savemore == "monthlysearch")
                {
                    int month = Convert.ToInt32(monthsearch);
                    int year = indianTime.Year;
                    var weeks = getWeekdatesandDates(month, year);
                    int days = DateTime.DaysInMonth(year, month);
                    int totalworkingdays = days - Convert.ToInt32(weeks.Count);
                    string txtdate = "00-" + month + "-" + year;
                    using (NpgsqlConnection connection = new NpgsqlConnection())
                    {
                        connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                        connection.Open();

                        string sql = "select username,name,count(*) as id from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,9)=SUBSTRING('" + txtdate + "',3,9) group by name,username order by name";
                        NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                        NpgsqlDataReader dr;
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            EmployeeAttendance p = new EmployeeAttendance();
                            p.totalworkingdays = totalworkingdays;
                            p.totalpresent = Convert.ToInt32(dr["id"].ToString());
                            p.totalabsent = totalworkingdays - Convert.ToInt32(dr["id"].ToString());
                            p.name = Convert.ToString(dr["name"].ToString());
                            p.username = Convert.ToString(dr["username"].ToString());
                            p.monthname = monthsearch;
                            Attendancedata1.Add(p);
                        }
                        Attendancedata = Attendancedata1;
                        dr.Close();

                        connection.Close();
                    }
                }
                else
                {
                    if (monthsearch == "0")
                    {
                        int month = indianTime.Month;
                        int year = indianTime.Year;
                        var weeks = getWeekdatesandDates(month, year);
                        int days = DateTime.DaysInMonth(year, month);
                        int totalworkingdays = days - Convert.ToInt32(weeks.Count);
                        using (NpgsqlConnection connection = new NpgsqlConnection())
                        {
                            connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                            connection.Open();

                            string sql = "select name,username,count(*) as id from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,9)=SUBSTRING('" + indianTime.ToString("dd-MM-yyyy") + "',3,9) group by name,username order by name";
                            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                            NpgsqlDataReader dr;
                            dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                EmployeeAttendance p = new EmployeeAttendance();
                                p.totalworkingdays = totalworkingdays;
                                p.totalpresent = Convert.ToInt32(dr["id"].ToString());
                                p.totalabsent = totalworkingdays - Convert.ToInt32(dr["id"].ToString());
                                p.name = Convert.ToString(dr["name"].ToString());
                                p.username = Convert.ToString(dr["username"].ToString());
                                p.monthname = Convert.ToString(month);
                                Attendancedata1.Add(p);
                            }
                            Attendancedata = Attendancedata1;
                            dr.Close();

                            connection.Close();

                        }
                    }
                    else
                    {
                        int month = Convert.ToInt32(monthsearch);
                        int year = indianTime.Year;
                        var weeks = getWeekdatesandDates(month, year);
                        int days = DateTime.DaysInMonth(year, month);
                        int totalworkingdays = days - Convert.ToInt32(weeks.Count);
                        string txtdate = "00-" + month + "-" + year;
                        using (NpgsqlConnection connection = new NpgsqlConnection())
                        {
                            connection.ConnectionString = "Server=103.118.157.29;Port=5432;Database=attendance;User Id=postgres;Password=princetonhive@123;";
                            connection.Open();

                            string sql = "select username,name,count(*) as id from employee inner join login on login.username=employee.employeeid where timein IS NOT NULL and SUBSTRING(date,3,9)=SUBSTRING('" + txtdate + "',3,9) group by name,username order by name";
                            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                            NpgsqlDataReader dr;
                            dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                EmployeeAttendance p = new EmployeeAttendance();
                                p.totalworkingdays = totalworkingdays;
                                p.totalpresent = Convert.ToInt32(dr["id"].ToString());
                                p.totalabsent = totalworkingdays - Convert.ToInt32(dr["id"].ToString());
                                p.name = Convert.ToString(dr["name"].ToString());
                                p.username = Convert.ToString(dr["username"].ToString());
                                p.monthname = monthsearch;
                                Attendancedata1.Add(p);
                            }
                            Attendancedata = Attendancedata1;
                            dr.Close();

                            connection.Close();
                        }
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
                        row.CreateCell(2).SetCellValue("Working Days");
                        row.CreateCell(3).SetCellValue("Present");
                        row.CreateCell(4).SetCellValue("Absent");

                        foreach (var segment in Attendancedata)
                        {
                            row = excelSheet.CreateRow(i);
                            row.CreateCell(0).SetCellValue(i);
                            row.CreateCell(1).SetCellValue(segment.name);
                            row.CreateCell(2).SetCellValue(segment.totalworkingdays);
                            row.CreateCell(3).SetCellValue(segment.totalpresent);
                            row.CreateCell(4).SetCellValue(segment.totalabsent);

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
            return Page();
        }
    }
}