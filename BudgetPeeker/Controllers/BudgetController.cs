using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ServiceStack.Text;
//using System.Threading.Tasks;
using BudgetPeeker.Models;
using BudgetPeeker.Persistence;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.EntityFrameworkCore.Query;
//using Microsoft.IdentityModel.Tokens;
using ServiceStack;

namespace BudgetPeeker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetPeekerDbContext _context;
        //
        // Limit page results to 100 records per page
        private int _resultsPerPage = 100;

        public BudgetController(BudgetPeekerDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //
            // Get dropdown models from distinct categories in
            var fiscalYear = _context.ApprovedBudget
                .OrderBy(ab => ab.Year)
                .Select(ab => ab.Year)
                .Distinct()
                .ToList();
            ViewBag.FiscalYearList = new SelectList(fiscalYear);
            
            
            var operatingUnit = _context.ApprovedBudget
                .OrderBy(ab => ab.OperatingUnitDescription)
                .Select(ab => ab.OperatingUnitDescription)
                .Distinct()
                .ToList();
            ViewBag.OperatingUnitList = new SelectList(operatingUnit);

            
            var accountCategory = _context.ApprovedBudget
                .OrderBy(ab => ab.AccountCategory)
                .Select(ab => ab.AccountCategory)
                .Distinct()
                .ToList();
            ViewBag.AccountCategoryList = new SelectList(accountCategory);
            
            
            var departmentDivision = _context.ApprovedBudget
                .OrderBy(ab => ab.DepartmentDivision)
                .Select(ab => ab.DepartmentDivision)
                .Distinct()
                .ToList();
            ViewBag.DepartmentDivisionList = new SelectList(departmentDivision);
            
            
            return View();
        }

        [HttpGet]
        public IActionResult Results([Bind("FiscalYear,OperatingUnit,AccountCategory,DepartmentDivision,OrderBudgetBy,Page")]BudgetViewModel budgetViewModel)
        {
            Console.WriteLine("------------- BudgetViewModel -----------");
            Console.WriteLine($"FiscalYear: {budgetViewModel.FiscalYear}");
            Console.WriteLine($"AccountCategory: {budgetViewModel.AccountCategory}");
            Console.WriteLine($"DepartmentDivision: {budgetViewModel.DepartmentDivision}");
            Console.WriteLine($"OperatingUnit: {budgetViewModel.OperatingUnit}");
            
            if (!ModelState.IsValid)
                return BadRequest("Uh oh... looks like something went wrong on our end :(");
            
            //
            // Update order selector
            var orderBy = new List<string> {"Default", "Ascending Order", "Descending Order"};
            ViewBag.OrderBudgetByList = new SelectList(orderBy);

            var nullSelectors = new List<string> {"All", "Select", "No"};

            var useFiscalYear = !nullSelectors.Contains(budgetViewModel.FiscalYear.Split(" ")[0]); // budgetViewModel.FiscalYear.Split(" ")[0] != "All" && budgetViewModel.FiscalYear.Split(" ")[0] != "Select" && budgetViewModel.FiscalYear.Split(" ")[0] != "No";
            var useAccountCategory = budgetViewModel.AccountCategory.Split(" ")[0] != "All" && budgetViewModel.AccountCategory.Split(" ")[0] != "Select" && budgetViewModel.AccountCategory.Split(" ")[0] != "No";
            var useOperatingUnit = budgetViewModel.OperatingUnit.Split(" ")[0] != "All" && budgetViewModel.OperatingUnit.Split(" ")[0] != "Select" && budgetViewModel.OperatingUnit.Split(" ")[0] != "No";
            var useDepartmentDivision = budgetViewModel.DepartmentDivision.Split(" ")[0] != "All" && budgetViewModel.DepartmentDivision.Split(" ")[0] != "Select" && budgetViewModel.DepartmentDivision.Split(" ")[0] != "No";

            //
            // Grab model from approved budget table
            var model = _context.ApprovedBudget.AsQueryable();

            //
            // Filter query based on user selected fields
            if (useFiscalYear)
                model = (from m in model
                    where m.Year == budgetViewModel.FiscalYear
                    select m);

            if (useAccountCategory)
                model = (from m in model
                    where m.AccountCategory == budgetViewModel.AccountCategory
                    select m);

            if (useDepartmentDivision)
                model = (from m in model 
                    where m.DepartmentDivision == budgetViewModel.DepartmentDivision
                    select m);

            if (useOperatingUnit)
                model = (from m in model
                    where m.OperatingUnitDescription == budgetViewModel.OperatingUnit
                    select m);
            
            // 
            // Get results based on user order selection
            var results = model;
            
            
            var filteredFiscalYear = results.OrderBy(r => r.Year).Select(r => r.Year).Distinct().ToList();
            var filteredAccountCategory = results.OrderBy(r => r.AccountCategory).Select(r => r.AccountCategory).Distinct().ToList();
            var filteredDepartmentDivision = results.OrderBy(r => r.DepartmentDivision).Select(r => r.DepartmentDivision).Distinct().ToList();
            var filteredOperatingUnit = results.OrderBy(r => r.OperatingUnitDescription).Select(r => r.OperatingUnitDescription).Distinct().ToList();
            
            //
            // Alter order based on user selection
            switch (budgetViewModel.OrderBudgetBy)
            {
                case "Ascending Order":
                    results = results.OrderBy(m => m.BudgetAmount);
                    break;
                case "Descending Order":
                    results = results.OrderByDescending(m => m.BudgetAmount);
                    break;
            }  
            
            //
            // Page results if greater than 100 records
            var count = results.Count();
            if (count > _resultsPerPage)
            {
                results = results.Skip(_resultsPerPage * (budgetViewModel.Page - 1)).Take(_resultsPerPage);
            }

            //
            // Get return results count and number of pages, initialize query list
            var numberOfPages = count / _resultsPerPage == 0 ? 1 : count / _resultsPerPage + 1;
            var budgetSum = 0;
            
            //
            // Integer overflow if all data selected
            if (useAccountCategory || useFiscalYear || useDepartmentDivision || useOperatingUnit)
                budgetSum = model.Sum(m => m.BudgetAmount ?? 0);

            //
            // Instantiate input selectors and lists for filters
            var inputSelectors = new BudgetViewModel
            {
                AccountCategory = budgetViewModel.AccountCategory,
                DepartmentDivision = budgetViewModel.DepartmentDivision,
                FiscalYear = budgetViewModel.FiscalYear,
                OperatingUnit = budgetViewModel.OperatingUnit,
                OrderBudgetBy = budgetViewModel.OrderBudgetBy,
                Page = budgetViewModel.Page,
            };

            var filteredBudgetLists = new FilteredBudgetLists()
            {
                FilteredFiscalYearList = filteredFiscalYear,
                FilteredAccountCategoryList = filteredAccountCategory,
                FilteredDepartmentDivisionList = filteredDepartmentDivision,
                FilteredOperatingUnitList = filteredOperatingUnit
            };
            
            //
            // Instantiate return results
            var result = new BudgetResultsViewModel()
            {
                QueryResults = results.ToList(),
                InputSelectors = inputSelectors,
                ResultsCount = count,
                NumberOfPages = numberOfPages,
                FilteredBudgetLists = filteredBudgetLists,
                TotalBudget = budgetSum
            };

            //
            // Add budget sum if not looking at all data
//            if (budgetSum > 0)
//                result.TotalBudget = budgetSum;
            Console.WriteLine(result.TotalBudget);
                
            ViewBag.BudgetViewModel = budgetViewModel;

            return View(result);
        }
        
        
        //
        // Export query to CSV
        public IActionResult ExportToCsv(List<ApprovedBudget> data)
        {
            var csvData = data.ToCsv();
            
            return Ok(csvData);
        }
        
        //
        // Convert budget object to csv for exporting
        public static string BudgetToCsv(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Value can not be null or Nothing!");
            }

            var sb = new StringBuilder();
            var t = obj.GetType();
            var propertyInfo = t.GetProperties();

            for (int index = 0; index < propertyInfo.Length; index++)
            {
                sb.Append(propertyInfo[index].GetValue(obj,null));

                if (index < propertyInfo.Length - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }
        
    }
}