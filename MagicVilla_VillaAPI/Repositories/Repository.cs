﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext context;
		internal DbSet<T> dbSet;
		public Repository(ApplicationDbContext _context)
		{
			context = _context;
			//context.VillaNumbers.Include(v => v.Villa).ToList();
			this.dbSet = context.Set<T>();
		}
		public async Task CreateAsync(T entity)
		{
			await dbSet.AddAsync(entity);
			await SaveAsync();
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if (!tracked)
			{
				query = query.AsNoTracking();
			}

			if (filter != null)
			{
				query = query.Where(filter);
			}
			if(includeProperties != null)
			{
				foreach(var includePrep in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includePrep);
				}
			}
			return await query.FirstOrDefaultAsync();
		}

		public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}
            if (includeProperties != null)
            {
                foreach (var includePrep in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))

                {
                    query = query.Include(includePrep);
                }
            }
            return await query.ToListAsync();
		}

		public async Task RemoveAsync(T entity)
		{
			context.Remove(entity);
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await context.SaveChangesAsync();
		}

	}
}
