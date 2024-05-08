using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repositories
{
	public class VillaRepository :Repository<Villa> , IVillaRepository
	{
		private readonly ApplicationDbContext context;
		public VillaRepository(ApplicationDbContext _context) : base(_context)
		{
			context = _context;
		}

		public async Task<Villa> UpdateAsync(Villa entity)
		{
			entity.UpdatedDate = DateTime.Now;

			context.Villas.Update(entity);
			await context.SaveChangesAsync();
			return entity;
		}
	}
}
