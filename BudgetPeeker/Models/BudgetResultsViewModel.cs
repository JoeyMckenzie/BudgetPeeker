using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BudgetPeeker.Models
{
    public class BudgetResultsViewModel
    {
        public List<BudgetModel> QueryResults { get; set; }
        public BudgetViewModel InputSelectors { get; set; }
        public BudgetViewModel PreviousResults { get; set; }
        public int ResultsCount { get; set; }
        public int? TotalBudget { get; set; }
        [Display(Name = "Number of pages")]
        public int NumberOfPages { get; set; }
        public int Page { get; set; }
        public Dictionary<int,IQueryable<ApprovedBudget>> PageQueryResults { get; set; }
        [Display(Name = "Order Budget By")] 
        public string OrderBudgetBy { get; set; }
        
        //
        // Filtered values
        public FilteredBudgetLists FilteredBudgetLists { get; set; }
        public string FilteredFiscalYear { get; set; }
        public string FilteredAccountCategory { get; set; }
        public string FilteredDepartmentDivision { get; set; }
        public string FilteredOperatingUnit { get; set; }
    }
}