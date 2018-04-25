using System;
using System.Collections.Generic;

namespace BudgetPeeker.Models
{
    public class ApprovedBudget
    {
        public int Id { get; set; }
        public string Activity { get; set; }
        public string Version { get; set; }
        public string Year { get; set; }
        public string AllOperUnits { get; set; }
        public string OperUnitGroup { get; set; }
        public string OperatingUnit { get; set; }
        public string OperatingUnitDescription { get; set; }
        public string AllDepts { get; set; }
        public string DepartmentOpUnit { get; set; }
        public string DepartmentDivision { get; set; }
        public string Deptid { get; set; }
        public string DepartmentDescription { get; set; }
        public string AllFunds { get; set; }
        public string FundGroup { get; set; }
        public string FundSummary { get; set; }
        public string Fund { get; set; }
        public string FundDescription { get; set; }
        public string AllAccounts { get; set; }
        public string Net { get; set; }
        public string Exprev { get; set; }
        public string OjbectClass { get; set; }
        public string AccountCategory { get; set; }
        public string Account { get; set; }
        public string AccountDescription { get; set; }
        public int? BudgetAmount { get; set; }
    }
}
