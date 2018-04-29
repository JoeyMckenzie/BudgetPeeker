namespace BudgetPeeker.Models
{
    public class FilteredBudgetViewModel : BudgetViewModel
    {
        public int FilteredFiscalYearCount { get; set; }
        public int FilteredAccountCategoryCount { get; set; }
        public int FilteredDepartmentDivisionCount { get; set; }
        public int FilteredFOperatingUnitCount { get; set; }
    }
}