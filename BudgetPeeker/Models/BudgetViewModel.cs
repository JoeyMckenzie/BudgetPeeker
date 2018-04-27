using System.ComponentModel.DataAnnotations;

namespace BudgetPeeker.Models
{
    public class BudgetViewModel
    {
        [Display(Name = "Fiscal Year")]
        public string FiscalYear { get; set; }
        [Display(Name = "Operating Unit")]
        public string OperatingUnit { get; set; }
        [Display(Name = "Account Category")]
        public string AccountCategory { get; set; }
        [Display(Name = "Department Division")]
        public string DepartmentDivision { get; set; }
        public int Page { get; set; }
        public string OrderBudgetBy { get; set; }
    }
}