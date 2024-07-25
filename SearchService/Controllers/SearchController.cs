using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems(string searchTerm, int pageNumber = 1, int pageSize = 4)
        {
            var query = DB.PagedSearch<Item>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }

            query.PageNumber(pageNumber);

            query.PageSize(pageSize);


            query.Sort(x => x.Ascending(a => a.Make));

            var results = await query.ExecuteAsync();

            return Ok(new
            {
                results = results.Results,
                pageCount = results.PageCount,
                totalCount = results.TotalCount
            });
        }
    }
}