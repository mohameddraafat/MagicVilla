using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;

namespace MagicVilla_VillaAPI.Repositories
{
	public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
	{
		private readonly ApplicationDbContext context;
        public VillaNumberRepository(ApplicationDbContext _context) :base(_context)
        {
			context = _context;
        }
        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
		{
			context.Update(entity);
			await context.SaveChangesAsync();
			return entity;
		}
	}
}
