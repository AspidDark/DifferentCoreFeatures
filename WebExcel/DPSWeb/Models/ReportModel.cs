using DPSWeb.DateSelector;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DPSWeb.Models
{
    public class ReportModel
    {
        public string title;
        public SelectList year;
        public Month month;
        public ReportModel()
        {
            year = new SelectList(new List<int> 
            { 
                DateTime.Now.Year, 
                DateTime.Now.AddYears(-1).Year, 
                DateTime.Now.AddYears(-2).Year, 
                DateTime.Now.AddYears(-3).Year
            });
        }
    }
}
