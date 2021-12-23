using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Travel.Data.Contexts;
using Travel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Travel.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourPackageController : ControllerBase
    {
        private readonly TravelDbContext context;
        public TourPackageController(TravelDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(context.TourPackages);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourPackage tourPackage)
        {
            await context.TourPackages.AddAsync(tourPackage);
            await context.SaveChangesAsync();
            return Ok(tourPackage);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tourPackage = await context.TourPackages.SingleOrDefaultAsync(tp => tp.Id == id);
            if (tourPackage is null)
            {
                return NotFound();
            }
            context.TourPackages.Remove(tourPackage);
            await context.SaveChangesAsync();
            return Ok(tourPackage);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TourPackage tourPackage)
        {
            context.Update(tourPackage);
            await context.SaveChangesAsync();
            return Ok(tourPackage);
        }

    }
}
