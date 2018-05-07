using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace BudgetPeeker.Models
{
    public class BudgetResultsViewModel
    {
        public List<BudgetModel> QueryResults { get; set; }
        public BudgetViewModel InputSelectors { get; set; }
        public int ResultsCount { get; set; }
        public int? TotalBudget { get; set; }
        [Display(Name = "Number of pages")]
        public int NumberOfPages { get; set; }
        public int Page { get; set; }
        [Display(Name = "Order budget by")] 
        public string OrderBudgetBy { get; set; }
        [Display(Name = "Sort fields by")]
        public string SortBy { get; set; }
        public FilteredBudgetLists FilteredBudgetLists { get; set; }
    }
}