using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EFDataAccess.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;

namespace EFDemoWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PeopleContext _db;

        public IndexModel(ILogger<IndexModel> logger, PeopleContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet()
        {
            LoadSampleData();

            var people = _db.People
                .Include(a => a.Addresses)
                .Include(e => e.EmailAddresses)
                .Where(x=>ApprovedAge(x.Age)) //-in code
                .Where(x=>x.Age>18); //in base
            //var sql = ((System.Data.Objects.ObjectQuery)people) //EF not core!!!
            //https://entityframeworkcore.com/knowledge-base/37527783/get-sql-code-from-an-ef-core-query
            //https://www.sqlservercentral.com/forums/topic/nvarchar4000-and-performance
            //.ToTraceString();
            // .Where();
            //  .ToList();
        }

        private bool ApprovedAge(int age)
        {
            return age > 18;
        }

        private void LoadSampleData()
        {
            if (_db.People.Count() == 0)
            { 
            //exec sp_executesql=>
            }
        
        }

       
    }
}
