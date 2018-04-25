using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetPeeker.Models;
using BudgetPeeker.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;

namespace BudgetPeeker.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetPeekerDbContext _context;
        //
        // Limit page results to 50 records per page
        private int _resultsPerPage = 100;

        public BudgetController(BudgetPeekerDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //
            // Get dropdown models from distinct categories in
            // ApprovedBudget database and add to queue
//            var dropdownModels = new Queue<SelectList>();
            
            var fiscalYear = _context.ApprovedBudget
                .OrderBy(ab => ab.Year)
                .Select(ab => ab.Year)
                .Distinct()
                .ToList();
            ViewBag.FiscalYearList = new SelectList(fiscalYear);
//            dropdownModels.Enqueue(new SelectList(fiscalYear));

            var operatingUnit = _context.ApprovedBudget
                .OrderBy(ab => ab.OperatingUnitDescription)
                .Select(ab => ab.OperatingUnitDescription)
                .Distinct()
                .ToList();
            ViewBag.OperatingUnitList = new SelectList(operatingUnit);
//            dropdownModels.Enqueue(new SelectList(operatingUnit));

            var accountCategory = _context.ApprovedBudget
                .OrderBy(ab => ab.AccountCategory)
                .Select(ab => ab.AccountCategory)
                .Distinct()
                .ToList();
            ViewBag.AccountCategoryList = new SelectList(accountCategory);
//            dropdownModels.Enqueue(new SelectList(accountCategory));
            
            var departmentDivision = _context.ApprovedBudget
                .OrderBy(ab => ab.DepartmentDivision)
                .Select(ab => ab.DepartmentDivision)
                .Distinct()
                .ToList();
            ViewBag.DepartmentDivisionList = new SelectList(departmentDivision);
//            dropdownModels.Enqueue(new SelectList(departmentDivision));
            
            
            //
            // Pass dropdown queue options to ViewBag
//            ViewBag.DropdownModels = dropdownModels;
            
            return View();
        }

        [HttpPost]
        public IActionResult Results([Bind("FiscalYear,OperatingUnit,AccountCategory,DepartmentDivision,OrderBudgetBy,Page")]BudgetViewModel budgetViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Uh oh... looks like something went wrong on our end :(");
            
            //
            // Update order selector
            var orderBy = new List<string> {"Default", "Ascending Order", "Descending Order"};
            ViewBag.OrderBudgetByList = new SelectList(orderBy);

            var useFiscalYear = budgetViewModel.FiscalYear.Split(" ")[0] != "All";
            var useAccountCategory = budgetViewModel.AccountCategory.Split(" ")[0] != "All";
            var useOperatingUnit = budgetViewModel.OperatingUnit.Split(" ")[0] != "All";
            var useDepartmentDivision = budgetViewModel.DepartmentDivision.Split(" ")[0] != "All";

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
            // Instantiate return results
            var result = new BudgetResultsViewModel()
            {
                QueryResults = results.ToList(),
                InputSelectors = new BudgetViewModel
                {
                    AccountCategory = budgetViewModel.AccountCategory,
                    DepartmentDivision = budgetViewModel.DepartmentDivision,
                    FiscalYear = budgetViewModel.FiscalYear,
                    OperatingUnit = budgetViewModel.OperatingUnit,
                    OrderBudgetBy = budgetViewModel.OrderBudgetBy,
                    Page = budgetViewModel.Page
                },
                ResultsCount = count,
                NumberOfPages = numberOfPages,
            };

            //
            // Add budget sum if not looking at all data
            if (budgetSum > 0)
                result.TotalBudget = budgetSum;
                
            ViewBag.BudgetViewModel = budgetViewModel;

            return View(result);
        }
    }
}