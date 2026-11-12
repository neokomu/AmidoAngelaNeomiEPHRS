using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPHRS
{
    // ===== Data Model: 10 columns =====
    public class Employee
    {
        public int Id { get; set; }                 // 1
        public string FirstName { get; set; } = ""; // 2
        public string LastName { get; set; } = "";  // 3
        public string Department { get; set; } = ""; // 4
        public string JobTitle { get; set; } = "";  // 5
        public decimal MonthlySalary { get; set; }  // 6
        public double TaxRate { get; set; }         // 7 (0.12 = 12%)
        public DateTime HireDate { get; set; }      // 8
        public bool IsActive { get; set; }          // 9
        public string Email { get; set; } = "";     // 10

        // Derived (not a column): net pay after tax
        public decimal NetSalary => MonthlySalary - (MonthlySalary * (decimal)TaxRate);
    }

    public class Program
    {
        // The list starts EMPTY (0 pre-defined records)
        private static readonly List<Employee> employees = new List<Employee>();
        private static int nextId = 1001; // auto-increment starting at 1001

        // >>> This is the line you will change on both branches to create the merge conflict <<<
        private const string AppTitle = "EPHRS - ANGELA NEOMI EDITION";

        public static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = (Console.ReadLine() ?? "").Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": CreateEmployee(); break;
                    case "2": ReadAllEmployees(); break;
                    case "3": SearchEmployees(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "6": running = false; Console.WriteLine("Goodbye!"); break;
                    default: Console.WriteLine("Invalid choice. Try again."); break;
                }
            }
        }

        private static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine(AppTitle);
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Create Employee");
            Console.WriteLine("2. Read / View All Employees");
            Console.WriteLine("3. Search / Filter Employees");
            Console.WriteLine("4. Update Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
        }

        // ===================== CREATE =====================
        private static void CreateEmployee()
        {
            Console.WriteLine("--- CREATE EMPLOYEE ---");
            var emp = new Employee
            {
                Id = nextId++,
                FirstName = ReadString("First Name: "),
                LastName = ReadString("Last Name: "),
                Department = ReadString("Department (IT/HR/Sales/Finance): "),
                JobTitle = ReadString("Job Title: "),
                MonthlySalary = ReadDecimal("Monthly Salary: "),
                TaxRate = ReadDouble("Tax Rate (e.g., 0.12 for 12%): ", 0, 1),
                HireDate = ReadDate("Hire Date (yyyy-MM-dd): "),
                IsActive = ReadBool("Is Active? (y/n): "),
                Email = ReadString("Email: ")
            };

            employees.Add(emp);
            Console.WriteLine($"Employee added successfully with Id {emp.Id}.");
        }

        // ===================== READ =====================
        private static void ReadAllEmployees()
        {
            Console.WriteLine("--- ALL EMPLOYEES ---");
            if (!employees.Any())
            {
                Console.WriteLine("No records found. The list is empty.");
                return;
            }

            PrintTable(employees.OrderBy(e => e.Id));

            // LINQ aggregates
            Console.WriteLine();
            Console.WriteLine($"Total Employees   : {employees.Count()}");
            Console.WriteLine($"Active Employees  : {employees.Count(e => e.IsActive)}");
            Console.WriteLine($"Total Monthly Pay : {employees.Sum(e => e.MonthlySalary):N2}");
            Console.WriteLine($"Average Salary    : {employees.Average(e => e.MonthlySalary):N2}");
        }

        // ===================== SEARCH / FILTER =====================
        private static void SearchEmployees()
        {
            Console.WriteLine("--- SEARCH / FILTER ---");
            if (!employees.Any()) { Console.WriteLine("No records to search."); return; }

            Console.WriteLine("1. By Id");
            Console.WriteLine("2. By Name (first or last)");
            Console.WriteLine("3. By Department");
            Console.WriteLine("4. By Active Status");
            Console.WriteLine("5. By Minimum Salary");
            Console.Write("Choose filter: ");
            string choice = (Console.ReadLine() ?? "").Trim();

            IEnumerable<Employee> results;

            switch (choice)
            {
                case "1":
                    int id = ReadInt("Enter Id: ");
                    results = employees.Where(e => e.Id == id);
                    break;
                case "2":
                    string name = ReadString("Enter name keyword: ");
                    results = employees.Where(e =>
                        e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "3":
                    string dept = ReadString("Enter department: ");
                    results = employees.Where(e =>
                        e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase));
                    break;
                case "4":
                    bool active = ReadBool("Active employees? (y = active, n = inactive): ");
                    results = employees.Where(e => e.IsActive == active);
                    break;
                case "5":
                    decimal min = ReadDecimal("Minimum monthly salary: ");
                    results = employees.Where(e => e.MonthlySalary >= min)
                                       .OrderByDescending(e => e.MonthlySalary);
                    break;
                default:
                    Console.WriteLine("Invalid filter.");
                    return;
            }

            var list = results.ToList();
            if (list.Count == 0) Console.WriteLine("No matching records found.");
            else PrintTable(list);
        }

        // ===================== UPDATE =====================
        private static void UpdateEmployee()
        {
            Console.WriteLine("--- UPDATE EMPLOYEE ---");
            if (!employees.Any()) { Console.WriteLine("No records to update."); return; }

            int id = ReadInt("Enter Id of employee to update: ");
            Employee? emp = employees.FirstOrDefault(e => e.Id == id);

            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            Console.WriteLine("Current record:");
            PrintTable(new[] { emp });
            Console.WriteLine("Press ENTER to keep the current value.");

            string input;

            Console.Write($"First Name [{emp.FirstName}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "") emp.FirstName = input;

            Console.Write($"Last Name [{emp.LastName}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "") emp.LastName = input;

            Console.Write($"Department [{emp.Department}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "") emp.Department = input;

            Console.Write($"Job Title [{emp.JobTitle}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "") emp.JobTitle = input;

            Console.Write($"Monthly Salary [{emp.MonthlySalary}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "" && decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0)
                emp.MonthlySalary = sal;

            Console.Write($"Tax Rate [{emp.TaxRate}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "" && double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
                emp.TaxRate = tax;

            Console.Write($"Hire Date [{emp.HireDate:yyyy-MM-dd}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "" && DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hd))
                emp.HireDate = hd;

            Console.Write($"Is Active [{(emp.IsActive ? "y" : "n")}] (y/n): ");
            input = (Console.ReadLine() ?? "").Trim().ToLower();
            if (input == "y") emp.IsActive = true;
            else if (input == "n") emp.IsActive = false;

            Console.Write($"Email [{emp.Email}]: ");
            input = (Console.ReadLine() ?? "").Trim();
            if (input != "") emp.Email = input;

            Console.WriteLine("Employee updated successfully.");
            PrintTable(new[] { emp });
        }

        // ===================== DELETE =====================
        private static void DeleteEmployee()
        {
            Console.WriteLine("--- DELETE EMPLOYEE ---");
            if (!employees.Any()) { Console.WriteLine("No records to delete."); return; }

            int id = ReadInt("Enter Id of employee to delete: ");
            Employee? emp = employees.FirstOrDefault(e => e.Id == id);

            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            PrintTable(new[] { emp });
            if (!ReadBool("Are you sure you want to delete this employee? (y/n): "))
            {
                Console.WriteLine("Delete cancelled.");
                return;
            }

            int removed = employees.RemoveAll(e => e.Id == id); // LINQ-style removal
            Console.WriteLine(removed > 0 ? "Employee deleted successfully." : "Nothing deleted.");
        }

        // ===================== DISPLAY =====================
        private static void PrintTable(IEnumerable<Employee> data)
        {
            foreach (var e in data)
            {
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine($"| Employee ID : {e.Id}");
                Console.WriteLine("+------------------------------------------------+");
                Console.WriteLine($"  First Name  : {e.FirstName}");
                Console.WriteLine($"  Last Name   : {e.LastName}");
                Console.WriteLine($"  Department  : {e.Department}");
                Console.WriteLine($"  Job Title   : {e.JobTitle}");
                Console.WriteLine($"  Salary      : {e.MonthlySalary:N2}");
                Console.WriteLine($"  Tax Rate    : {e.TaxRate:P0}");
                Console.WriteLine($"  Net Salary  : {e.NetSalary:N2}");
                Console.WriteLine($"  Hire Date   : {e.HireDate:yyyy-MM-dd}");
                Console.WriteLine($"  Status      : {(e.IsActive ? "Active" : "On Leave/Inactive")}");
                Console.WriteLine($"  Email       : {e.Email}");
                Console.WriteLine();
            }
        }

        // ===================== INPUT HELPERS =====================
        private static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();
                if (input != "") return input;
                Console.WriteLine("Input cannot be empty.");
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value)) return value;
                Console.WriteLine("Please enter a valid whole number.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
                    return value;
                Console.WriteLine("Please enter a valid non-negative amount.");
            }
        }

        private static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
                    return value;
                Console.WriteLine("Please enter a valid date (yyyy-MM-dd).");
            }
        }

        private static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim().ToLower();
                if (input == "y" || input == "yes") return true;
                if (input == "n" || input == "no") return false;
                Console.WriteLine("Please enter y or n.");
            }
        }
    }
}
