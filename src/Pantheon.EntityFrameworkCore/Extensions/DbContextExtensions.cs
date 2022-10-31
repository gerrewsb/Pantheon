using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pantheon.Abstractions.Contracts;
using Pantheon.EntityFrameworkCore.Abstractions.Bases;
using Pantheon.EntityFrameworkCore.Abstractions.Contracts;
using Pantheon.Exceptions;
using System.Net;

namespace Pantheon.EntityFrameworkCore.Extensions
{
	public static class DbContextExtensions
	{
		public static IQueryable<TEntity> GetAll<TEntity, TKey>(this DbContext context, IIncludeSpecifications<TEntity>? includeSpecifications = null)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			return includeSpecifications != null
				? context.Set<TEntity>().WithIncludes<TEntity, TKey>(includeSpecifications).AsNoTracking()
				: context.Set<TEntity>().AsNoTracking();
		}

		public static IQueryable<TEntity> GetById<TEntity, TKey>(this DbContext context, TKey id, IIncludeSpecifications<TEntity>? includeSpecifications = null)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			return includeSpecifications != null
				? context.Set<TEntity>().WithIncludes<TEntity, TKey>(includeSpecifications).AsNoTracking().Where(x => x.ID.Equals(id))
				: context.Set<TEntity>().AsNoTracking().Where(x => x.ID.Equals(id));
		}

		public static async Task<TEntity> CreateWithTransaction<TEntity, TKey>(this DbContext context, TEntity entity, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
			context.Set<TEntity>().Add(entity);
			await context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
			return entity;			
		}

		public static async Task<IEnumerable<TEntity>> CreateWithTransaction<TEntity, TKey>(this DbContext context, IList<TEntity> entities, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			await context.BulkInsertAsync(entities, cancellationToken: cancellationToken);
			return entities;
		}

		public static async Task UpdateWithTransaction<TEntity, TKey>(this DbContext context, TEntity entity, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
			context.Set<TEntity>().Update(entity);
			await context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		public static async Task UpdateWithTransaction<TEntity, TKey>(this DbContext context, IList<TEntity> entities, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			await context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
		}

		public static async Task DeleteWithTransaction<TEntity, TKey>(this DbContext context, TKey id, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
			TEntity? entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.ID.Equals(id), cancellationToken);

			if (entity == null)
			{
				throw new ApiException($"Couldn't delete {id} in DB.", nameof(id), HttpStatusCode.BadRequest);
			}

			context.Set<TEntity>().Remove(entity);
			await context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		public static async Task DeleteWithTransaction<TEntity, TKey>(this DbContext context, IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>
			where TKey : IEquatable<TKey>
		{
			IList<TEntity> entities = await context.Set<TEntity>()
				.Where(x => keys.Contains(x.ID))
				.ToListAsync(cancellationToken);

			if (entities?.Any() != true)
			{
				throw new ApiException($"Couldn't delete keys in DB.", nameof(keys), HttpStatusCode.BadRequest);
			}

			await context.BulkDeleteAsync(entities, cancellationToken: cancellationToken);
		}

		public static async Task ActivateWithTransaction<TEntity, TKey>(this DbContext context, TKey id, DateTime? validUntil = null, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>, IValidatable
			where TKey : IEquatable<TKey>
		{
			using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
			TEntity? entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.ID.Equals(id), cancellationToken);

			if (entity == null)
			{
				throw new ApiException($"The provided ID: {id} for type: {typeof(TEntity).Name} doesn't exist in the DB.", nameof(id), HttpStatusCode.BadRequest);
			}

			if (validUntil != null && (validUntil < DateTime.UtcNow || validUntil < entity.ValidFrom))
			{
				throw new ApiException("ValidUntil must be in the future and be later than the ValidFrom date.", nameof(validUntil), HttpStatusCode.BadRequest);
			}

			entity.ValidUntil = validUntil;
			context.Set<TEntity>().Update(entity);
			await context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		public static async Task ActivateWithTransaction<TEntity, TKey>(this DbContext context, IList<TKey> keys, DateTime? validUntil = null, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>, IValidatable
			where TKey : IEquatable<TKey>
		{
			IList<TEntity> entities = await context.Set<TEntity>()
				.Where(x => keys.Contains(x.ID))
				.ToListAsync(cancellationToken);

			if (entities?.Any() != true)
			{
				throw new ApiException($"Couldn't find keys in DB.", nameof(keys), HttpStatusCode.BadRequest);
			}

			foreach (var entity in entities)
			{
				if (validUntil != null && (validUntil < DateTime.UtcNow || validUntil < entity.ValidFrom))
				{
					throw new ApiException("ValidUntil must be in the future and be later than the ValidFrom date.", nameof(validUntil), HttpStatusCode.BadRequest);
				}

				entity.ValidUntil = validUntil;
			}

			await context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
		}

		public static async Task DeactivateWithTransaction<TEntity, TKey>(this DbContext context, TKey id, DateTime? validUntil = null, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>, IValidatable
			where TKey : IEquatable<TKey>
		{
			using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
			TEntity? entity = await context.Set<TEntity>().FirstOrDefaultAsync(x => x.ID.Equals(id), cancellationToken);

			if (entity == null)
			{
				throw new ApiException($"The provided ID: {id} for type: {typeof(TEntity).Name} doesn't exist in the DB.", nameof(id), HttpStatusCode.BadRequest);
			}

			if (validUntil != null && (validUntil < DateTime.UtcNow || validUntil < entity.ValidFrom))
			{
				throw new ApiException("ValidUntil can't be before ValidUntil.", nameof(validUntil), HttpStatusCode.BadRequest);
			}

			entity.ValidUntil = validUntil ?? (DateTime.Today >= entity.ValidFrom ? DateTime.Today : entity.ValidFrom);
			context.Set<TEntity>().Update(entity);
			await context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		public static async Task DeactivateWithTransaction<TEntity, TKey>(this DbContext context, IList<TKey> keys, DateTime? validUntil = null, CancellationToken cancellationToken = default)
			where TEntity : EntityBase<TKey>, IValidatable
			where TKey : IEquatable<TKey>
		{
			IList<TEntity> entities = await context.Set<TEntity>()
				.Where(x => keys.Contains(x.ID))
				.ToListAsync(cancellationToken);

			if (entities?.Any() != true)
			{
				throw new ApiException($"Couldn't find keys in DB.", nameof(keys), HttpStatusCode.BadRequest);
			}

			foreach (var entity in entities)
			{
				if (validUntil != null && (validUntil < DateTime.UtcNow || validUntil < entity.ValidFrom))
				{
					throw new ApiException("ValidUntil must be in the future and be later than the ValidFrom date.", nameof(validUntil), HttpStatusCode.BadRequest);
				}

				entity.ValidUntil = validUntil ?? (DateTime.Today >= entity.ValidFrom ? DateTime.Today : entity.ValidFrom);
			}

			await context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
		}
	}
}
