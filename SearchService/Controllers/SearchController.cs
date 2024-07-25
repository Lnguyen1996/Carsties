using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(i => i.Make))
            };

            query.PageNumber(searchParams.PageNumber);

            query.PageSize(searchParams.PageSize);

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