using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetPeeker.Models
{
    public class FilteredBudgetLists
    {
        public List<string> FilteredFiscalYearList { get; set; }
        public List<string> FilteredAccountCategoryList { get; set; }
        public List<string> FilteredOperatingUnitList { get; set; }
        public List<string> FilteredDepartmentDivisionList { get; set; }
    }
}