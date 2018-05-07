using System.Linq;
using BudgetPeeker.Models;

namespace BudgetPeeker.Persistence
{
    public static class SortResults
    {
        public static IQueryable<ApprovedBudget> SortByAscendingOrderBudgetByAscending(BudgetViewModel budgetViewModel, IQueryable<ApprovedBudget> query)
        {
            switch (budgetViewModel.SortBy)
            {
                case "Fiscal Year":
                    query = query.OrderBy(m => m.Year).ThenBy(m => m.BudgetAmount);
                    break;
                case "Department Division":
                    query = query.OrderBy(m => m.DepartmentDivision).ThenBy(m => m.BudgetAmount);
                    break;
                case "Operating Unit":
                    query = query.OrderBy(m => m.OperatingUnitDescription).ThenBy(m => m.BudgetAmount);
                    break;
                case "Account Category":
                    query = query.OrderBy(m => m.AccountCategory).ThenBy(m => m.BudgetAmount);
                    break;
                default:
                    query = query.OrderBy(m => m.BudgetAmount);
                    break;
            }

            return query;
        }
        
        public static IQueryable<ApprovedBudget> SortByAscendingOrderBudgetByDescending(BudgetViewModel budgetViewModel, IQueryable<ApprovedBudget> query)
        {
            switch (budgetViewModel.SortBy)
            {
                case "Fiscal Year":
                    query = query.OrderBy(m => m.Year).ThenByDescending(m => m.BudgetAmount);
                    break;
                case "Department Division":
                    query = query.OrderBy(m => m.DepartmentDivision).ThenByDescending(m => m.BudgetAmount);
                    break;
                case "Operating Unit":
                    query = query.OrderBy(m => m.OperatingUnitDescription).ThenByDescending(m => m.BudgetAmount);
                    break;
                case "Account Category":
                    query = query.OrderBy(m => m.AccountCategory).ThenByDescending(m => m.BudgetAmount);
                    break;
                default:
                    query = query.OrderByDescending(m => m.BudgetAmount);
                    break;
            }

            return query;
        }

        public static IQueryable<ApprovedBudget> SortByAscendingOrderBudgetByDefault(BudgetViewModel budgetViewModel, IQueryable<ApprovedBudget> query)
        {
            switch (budgetViewModel.SortBy)
            {
                case "Fiscal Year":
                    query = query.OrderBy(m => m.Year);
                    break;
                case "Department Division":
                    query = query.OrderBy(m => m.DepartmentDivision);
                    break;
                case "Operating Unit":
                    query = query.OrderBy(m => m.OperatingUnitDescription);
                    break;
                case "Account Category":
                    query = query.OrderBy(m => m.AccountCategory);
                    break;
            }

            return query;
        }
    }
}