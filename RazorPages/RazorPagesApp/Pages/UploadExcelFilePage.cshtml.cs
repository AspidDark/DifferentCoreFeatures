using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesApp.Pages
{
    /// <summary>
    ///    <label asp-for="SelectedValue" class="control-label"></label>
    ///    <input asp-for="SelectedValue" class="form-control" />
    ///    
    /// 
    ///    <select asp-for="SelectedValue" asp-items="Model.Values">
    ///        <option value = "" ></ option >
    ///     </ select >
    /// 
    /// </summary>
    public class UploadExcelFilePageModel : PageModel
    {
       
        public SelectList Values { get; set; }
        [BindProperty]
        public IFormFile FileUpload { get; set; }
        [BindProperty]
        public string SelectedValue { get; set; }

        public void OnGet()
        {
            IEnumerable<string> valuesTemp = new List<string> { "aaa", "BBB", "CCC" };

            Values = new SelectList( valuesTemp, "ZZZ");
        }
        public void OnPost()
        {
            var fileUpl = FileUpload;
            var tmp = SelectedValue;
            
            var tmp2 = 1;
        }
    }
}
