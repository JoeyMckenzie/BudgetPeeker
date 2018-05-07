using System;
using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BudgetPeeker.Models;
using BudgetPeeker.Persistence;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetPeeker.Controllers
{
    public class BudgetController : Controller
    {
        //
        // Limit page results to 50 records per page
        private const int ResultsPerPage = 50;
        private readonly BudgetPeekerDbContext _context;

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
        public IActionResult Results([Bind("FiscalYear,OperatingUnit,AccountCategory,DepartmentDivision,OrderBudgetBy,Page,SortBy")]BudgetViewModel budgetViewModel)
        {
            Console.WriteLine("----------");
            Console.WriteLine($"SortBy: {budgetViewModel.SortBy}");

            if (!ModelState.IsValid)
                return BadRequest("Uh oh... looks like something went wrong on our end :(");


            //
            // Update order selector
            var orderBy = new List<string> {"Default", "Ascending Order", "Descending Order"};
            var sortFieldsBy = new List<string> {"Default", "Fiscal Year", "Account Category", "Department Division", "Operating Unit"};
            ViewBag.OrderBudgetByList = new SelectList(orderBy);
            ViewBag.SortFieldsByList = new SelectList(sortFieldsBy);



            //
            // Specify if user is selecting unique fields
            var nullSelectors = new List<string> {"All", "Select", "No"};
            var useFiscalYear = !nullSelectors.Contains(budgetViewModel.FiscalYear.Split(" ")[0]);
            var useAccountCategory = !nullSelectors.Contains(budgetViewModel.AccountCategory.Split(" ")[0]);
            var useOperatingUnit = !nullSelectors.Contains(budgetViewModel.OperatingUnit.Split(" ")[0]);
            var useDepartmentDivision = !nullSelectors.Contains(budgetViewModel.DepartmentDivision.Split(" ")[0]);

            //
            // Grab model from approved budget table
            var model = _context.ApprovedBudget.AsQueryable();

            //
            // Filter query based on user selected fields
            if (useFiscalYear)
                model = from m in model
                    where m.Year == budgetViewModel.FiscalYear
                    select m;

            if (useAccountCategory)
                model = from m in model
                    where m.AccountCategory == budgetViewModel.AccountCategory
                    select m;

            if (useDepartmentDivision)
                model = from m in model
                    where m.DepartmentDivision == budgetViewModel.DepartmentDivision
                    select m;

            if (useOperatingUnit)
                model = from m in model
                    where m.OperatingUnitDescription == budgetViewModel.OperatingUnit
                    select m;


            //
            // Avoid integer overflow if all data selected
            var budgetSum = 0;
            if (useAccountCategory || useFiscalYear || useDepartmentDivision || useOperatingUnit)
                budgetSum = model.Sum(m => m.BudgetAmount ?? 0);


            // 
            // Get results based on user order selection
            var results = model;

            var filteredFiscalYear = results
                .OrderBy(r => r.Year)
                .Select(r => r.Year)
                .Distinct()
                .ToList();
            
            var filteredAccountCategory = results
                .OrderBy(r => r.AccountCategory)
                .Select(r => r.AccountCategory)
                .Distinct()
                .ToList();
            
            
            var filteredDepartmentDivision = results
                .OrderBy(r => r.DepartmentDivision)
                .Select(r => r.DepartmentDivision)
                .Distinct()
                .ToList();
            
            
            var filteredOperatingUnit = results
                .OrderBy(r => r.OperatingUnitDescription)
                .Select(r => r.OperatingUnitDescription)
                .Distinct()
                .ToList();


            //
            // Sort results by input selectors
            if (!string.IsNullOrEmpty(budgetViewModel.SortBy))
            {
                switch (budgetViewModel.OrderBudgetBy)
                {
                    case "Ascending Order":
                        results = SortResults.SortByAscendingOrderBudgetByAscending(budgetViewModel, results);
                        break;
                    case "Descending Order":
                        results = SortResults.SortByAscendingOrderBudgetByDescending(budgetViewModel, results);
                        break;
                    default:
                        results = SortResults.SortByAscendingOrderBudgetByDefault(budgetViewModel, results);
                        break;
                }
            }
            else
            {
                switch (budgetViewModel.OrderBudgetBy)
                {
                    case "Ascending Order":
                        results = results.OrderBy(r => r.BudgetAmount);
                        break;
                    case "Descending Order":
                        results = results.OrderByDescending(r => r.BudgetAmount);
                        break;
                }
            }
            

            //
            // Get results for CSV download
            var budgetResultsCsv = results.Select(r => new BudgetModel
            {
                Id = r.Id,
                FiscalYear = r.Year,
                AccountCategory = r.AccountCategory,
                DepartmentDivision = r.DepartmentDivision,
                OperatingUnit = r.OperatingUnitDescription,
                BudgetAmount = r.BudgetAmount
            }).ToList();


            //
            // Export to CSV
            using (TextWriter writer = new StreamWriter("./wwwroot/BudgetData.csv"))
            {
                var csv = new CsvWriter(writer);

                //
                // Write selected fields at top of CSV
                csv.WriteField(string.Concat("Fiscal Year: ", budgetViewModel.FiscalYear));
                csv.NextRecord();
                csv.WriteField(string.Concat("Department Division: ", budgetViewModel.DepartmentDivision));
                csv.NextRecord();
                csv.WriteField(string.Concat("Account Category: ", budgetViewModel.AccountCategory));
                csv.NextRecord();
                csv.WriteField(string.Concat("Operating Unit: ", budgetViewModel.OperatingUnit));
                csv.NextRecord();
                csv.WriteField(string.Concat("Budget Amount: ", budgetSum));
                csv.NextRecord();

                //
                // Write all records to CSV
                csv.WriteRecords(budgetResultsCsv);
            }

            //
            // Page results if greater than 50 records
            var count = results.Count();
            if (count > ResultsPerPage)
            {
                results = results.Skip(ResultsPerPage * (budgetViewModel.Page - 1)).Take(ResultsPerPage);
            }

            var budgetResults = results.Select(r => new BudgetModel
            {
                Id = r.Id,
                FiscalYear = r.Year,
                AccountCategory = r.AccountCategory,
                DepartmentDivision = r.DepartmentDivision,
                OperatingUnit = r.OperatingUnitDescription,
                BudgetAmount = r.BudgetAmount
            }).ToList();

            //
            // Get return results count and number of pages, initialize query list
            var numberOfPages = count / ResultsPerPage == 0 ? 1 : count / ResultsPerPage + 1;


            //
            // Instantiate input selectors
            var inputSelectors = new BudgetViewModel
            {
                AccountCategory = budgetViewModel.AccountCategory,
                DepartmentDivision = budgetViewModel.DepartmentDivision,
                FiscalYear = budgetViewModel.FiscalYear,
                OperatingUnit = budgetViewModel.OperatingUnit,
                OrderBudgetBy = budgetViewModel.OrderBudgetBy,
                SortBy = budgetViewModel.SortBy,
                Page = budgetViewModel.Page,
            };

            //
            // Instantiate lists for filters
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
                QueryResults = budgetResults,
                InputSelectors = inputSelectors,
                ResultsCount = count,
                NumberOfPages = numberOfPages,
                FilteredBudgetLists = filteredBudgetLists,
                TotalBudget = budgetSum
            };

            return View(result);
        }
    }
}
