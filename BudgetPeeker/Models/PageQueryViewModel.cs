using System.Linq;

namespace BudgetPeeker.Models
{
    public class PageQueryViewModel
    {
        public int Page { get; set; }
        public IQueryable<ApprovedBudget> BudgetQuery { get; set; }
    }
}