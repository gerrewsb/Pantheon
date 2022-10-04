using Microsoft.EntityFrameworkCore;
using Pantheon.Abstractions.Contracts;
using Pantheon.Exceptions;
using Pantheon.Extensions;
using Pantheon.Helpers;
using System.Linq.Expressions;
using System.Net;

namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// <para>Base repository with common functionality like GetAll, GetByID, Create, Update, Delete</para>
	/// <para><see cref="ISpecifications{TEntity}"/> can be passed in the methods to have some base includes or filtering</para>
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class RepositoryBase<TContext, TEntity, TKey> : IRepository<TEntity, TKey>
		where TContext : DbContext
		where TEntity : EntityBase<TKey>
		where TKey : IEquatable<TKey>
	{
		protected TContext Context { get; init; }

		public RepositoryBase(TContext context)
		{
			Context = context;
		}

		/// <summary>
		/// Method to get all entries for a given Entity
		/// </summary>
		/// <param name="baseSpecifications"></param>
		/// <returns>IEnumerable of type TEntity</returns>
		/// <exception cref="ApiException">If no entries are found for this Entity</exception>
		public async virtual Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity>? baseSpecifications = null)
		{
			try
			{
				IEnumerable<TEntity> entities = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.ToListAsync();

				if (entities?.Any() != true)
				{
					throw new ApiException($"No data found for this type: {typeof(TEntity).Name}.", nameof(entities), HttpStatusCode.NotFound);
				}

				return entities;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Get all entries for a given Entity but with a selector to only select a few properties or to map it to another model
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="baseSpecifications"></param>
		/// <returns>IEnumerable of type TResult</returns>
		/// <exception cref="ApiException">If no entries are found for this Entity</exception>
		public async virtual Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, ISpecifications<TEntity>? baseSpecifications = null)
		{
			try
			{
				IEnumerable<TResult> entities = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.Select(selector)
					.ToListAsync();

				if (entities?.Any() != true)
				{
					throw new ApiException($"No data found for this type: {typeof(TResult).Name}.", nameof(entities), HttpStatusCode.NotFound);
				}

				return entities;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Get all entries for a given Entity grouped by the expression and selector
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="groupByExpression"></param>
		/// <param name="groupBySelector"></param>
		/// <param name="baseSpecifications"></param>
		/// <returns>IEnumerable of type object</returns>
		/// <exception cref="ApiException">If no entries are found for this Entity</exception>
		public async virtual Task<IEnumerable<object>> GetAllAsync<TResult>(Expression<Func<TEntity, object>> groupByExpression, Expression<Func<IGrouping<object, TEntity>, TResult>> groupBySelector, ISpecifications<TEntity>? baseSpecifications = null)
		{
			try
			{
				IEnumerable<TResult> entities = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.GroupBy(groupByExpression)
					.Select(groupBySelector)
					.ToListAsync();

				if (entities?.Any() != true)
				{
					throw new ApiException($"No data found for this type: {typeof(TEntity).Name}.", nameof(entities), HttpStatusCode.NotFound);
				}

				return (IEnumerable<object>)entities;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Get a single entry for an Entity based on the ID
		/// </summary>
		/// <param name="id"></param>
		/// <param name="baseSpecifications"></param>
		/// <returns><see cref="TEntity"/></returns>
		/// <exception cref="ApiException">If no entry is found for this Entity</exception>
		public async virtual Task<TEntity> GetByIdAsync(TKey id, ISpecifications<TEntity>? baseSpecifications = null)
		{
			try
			{
				TEntity? entity = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.ID.Equals(id));

				return entity ?? throw new ApiException($"{typeof(TEntity).Name} with ID: {id} was not found in DB.", nameof(id), HttpStatusCode.NotFound); ;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Get a single entry for an Entity based on the ID and select the needed properties or map to another model
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="id"></param>
		/// <param name="selector"></param>
		/// <param name="baseSpecifications"></param>
		/// <returns><see cref="TResult"/></returns>
		/// <exception cref="ApiException">If no entry is found for this Entity</exception>
		public async virtual Task<TResult> GetByIdAsync<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector, ISpecifications<TEntity>? baseSpecifications = null)
		{
			try
			{
				var entity = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.ID.Equals(id));

				if (entity == null)
				{
					throw new ApiException($"{typeof(TEntity).Name} with ID: {id} was not found in DB.", nameof(id), HttpStatusCode.NotFound);
				}

				var result = entity.ToQueryable()
					.Select(selector)
					.FirstOrDefault();

				return result ?? throw new ApiException($"{typeof(TResult).Name} could not be resolved from corresponding entity.", nameof(id), HttpStatusCode.NotFound);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// <para>Get a single entry for an Entity based on an expression.</para>
		/// Only use this method if the ID is unknown
		/// </summary>
		/// <param name="baseSpecifications"></param>
		/// <returns>Returns the <see cref="TEntity"/> or an ApiException if no result is found.</returns>
		/// <exception cref="ApiException">If no entry is found for this Entity</exception>
		public async virtual Task<TEntity> GetFirstByExpressionAsync(ISpecifications<TEntity> baseSpecifications)
		{
			try
			{
				TEntity? entity = await SpecificationEvaluator<TEntity, TKey>.GetQuery(Context.Set<TEntity>()
					.AsQueryable(), baseSpecifications)
					.AsNoTracking()
					.FirstOrDefaultAsync();

				return entity ?? throw new ApiException($"{typeof(TEntity).Name} was not found in DB.", nameof(entity), HttpStatusCode.NotFound);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Create a new entry for the Entity
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>The newly created <see cref="TEntity"/></returns>
		/// <exception cref="ApiException"></exception>
		public async virtual Task<TEntity> CreateAsync(TEntity? entity)
		{
			try
			{
				if (entity == null)
				{
					throw new ApiException($"The provided {typeof(TEntity).Name} was null.", nameof(entity), HttpStatusCode.BadRequest);
				}

				Context.Set<TEntity>().Add(entity);
				await Context.SaveChangesAsync();

				return entity;
			}
			catch (DbUpdateConcurrencyException ex)
			{
				throw new ApiException(ex.Message, nameof(entity), ex, HttpStatusCode.BadRequest);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Update an existing entry of the Entity
		/// </summary>
		/// <param name="entity"></param>
		/// <exception cref="ApiException"></exception>
		public async virtual Task UpdateAsync(TEntity? entity)
		{
			try
			{
				if (entity == null)
				{
					throw new ApiException($"The provided {typeof(TEntity).Name} was null.", nameof(entity), HttpStatusCode.BadRequest);
				}

				Context.Set<TEntity>().Update(entity);
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				throw new ApiException(ex.Message, nameof(entity), ex, HttpStatusCode.BadRequest);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Delete an existing entry of the Entity
		/// </summary>
		/// <param name="id"></param>
		/// <exception cref="ApiException"></exception>
		public async virtual Task DeleteAsync(TKey id)
		{
			try
			{
				TEntity? entity = await Context.Set<TEntity>()
					.FirstOrDefaultAsync(x => x.ID.Equals(id));

				if (entity == null)
				{
					throw new ApiException($"The provided ID: {id} for type: {typeof(TEntity).Name} doesn't exist in the DB.", nameof(id), HttpStatusCode.BadRequest);
				}

				Context.Set<TEntity>().Remove(entity);
				await Context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				throw new ApiException(ex.Message, nameof(id), ex, HttpStatusCode.BadRequest);
			}
			catch
			{
				throw;
			}
		}
	}
}
