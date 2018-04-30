using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BudgetPeeker.Models;
using BudgetPeeker.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            if (!ModelState.IsValid)
                return BadRequest("Uh oh... looks like something went wrong on our end :(");
            
            //
            // Update order selector
            var orderBy = new List<string> {"Default", "Ascending Order", "Descending Order"};
            ViewBag.OrderBudgetByList = new SelectList(orderBy);

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
                csv.WriteRecords(budgetResultsCsv);
            }
            
            //
            // Page results if greater than 100 records
            var count = results.Count();
            if (count > _resultsPerPage)
            {
                results = results.Skip(_resultsPerPage * (budgetViewModel.Page - 1)).Take(_resultsPerPage);
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
            var numberOfPages = count / _resultsPerPage == 0 ? 1 : count / _resultsPerPage + 1;
            var budgetSum = 0;
            
            //
            // Integer overflow if all data selected
            if (useAccountCategory || useFiscalYear || useDepartmentDivision || useOperatingUnit)
                budgetSum = model.Sum(m => m.BudgetAmount ?? 0);

            //
            // Instantiate input selectors
            var inputSelectors = new BudgetViewModel
            {
                AccountCategory = budgetViewModel.AccountCategory,
                DepartmentDivision = budgetViewModel.DepartmentDivision,
                FiscalYear = budgetViewModel.FiscalYear,
                OperatingUnit = budgetViewModel.OperatingUnit,
                OrderBudgetBy = budgetViewModel.OrderBudgetBy,
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