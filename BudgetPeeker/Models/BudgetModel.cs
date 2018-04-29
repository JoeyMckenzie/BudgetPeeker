namespace BudgetPeeker.Models
{
    public class BudgetModel
    {
        public int Id { get; set; }
        public string FiscalYear { get; set; }
        public string OperatingUnit { get; set; }
        public string AccountCategory { get; set; }
        public string DepartmentDivision { get; set; }
        public int? BudgetAmount { get; set; }
    }
}