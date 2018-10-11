using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Web.Models
{
    public class FilterOptions
    {
        [FromQuery(Name = "search")]
        public string SearchPhrase { get; set; }

        [FromQuery(Name = "orderby")]
        public string OrderBy { get; set; }

        [FromQuery(Name = "pageindex")]
        [Required]
        public int? PageIndex { get; set; }

        [FromQuery(Name = "pagesize")]
        [Required]
        public int? PageSize { get; set; }
    }
}
