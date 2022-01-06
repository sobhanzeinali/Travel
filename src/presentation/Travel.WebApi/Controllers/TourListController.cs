using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Data.Contexts;
using Travel.Domain.Entities;
namespace Travel.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourListController : ControllerBase
    {
        private readonly TravelDbContext context;
        public TourListController(TravelDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(context.TourLists);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourList tourList)
        {
            await context.TourLists.AddAsync(tourList);
            await context.SaveChangesAsync();
            return Ok(tourList);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tourList = await context.TourLists.SingleOrDefaultAsync(tl => tl.Id == id);
            if (tourList is null)
            {
                return NotFound();
            }
            context.TourLists.Remove(tourList);
            await context.SaveChangesAsync();
            return Ok(tourList);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TourList tourList)
        {
            context.TourLists.Update(tourList);
            await context.SaveChangesAsync();
            return Ok(tourList);
        }
    }
}
