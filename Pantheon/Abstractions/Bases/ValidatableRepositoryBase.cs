using Microsoft.EntityFrameworkCore;
using Pantheon.Abstractions.Contracts;
using Pantheon.Exceptions;
using System.Net;

namespace Pantheon.Abstractions.Bases
{
	public class ValidatableRepositoryBase<TContext, TEntity, TKey> : RepositoryBase<TContext, TEntity, TKey>, IValidatableRepository<TEntity, TKey>
		where TContext : DbContext
		where TEntity : EntityBase<TKey>, IValidatable
		where TKey : IEquatable<TKey>
	{
		public ValidatableRepositoryBase(TContext context) : base(context)
		{ }

		/// <summary>
		/// <para>Activate an IValidatable Entity based on the ID</para>
		/// <para>if ValidUntil is passed, it needs to be after <see cref="DateTime.UtcNow"/> and be after ValidFrom</para>
		/// </summary>
		/// <param name="id">ID of the entity</param>
		/// <param name="validUntil">optional <see cref="DateTime"/> to determine until when the entity is active/valid</param>
		/// <exception cref="ApiException"></exception>
		public async virtual Task ActivateAsync(TKey id, DateTime? validUntil = null)
		{
			try
			{
				TEntity? entity = await Context.Set<TEntity>().FirstOrDefaultAsync(x => x.ID.Equals(id));

				if (entity == null)
				{
					throw new ApiException($"The provided ID: {id} for type: {typeof(TEntity).Name} doesn't exist in the DB.", nameof(id), HttpStatusCode.BadRequest);
				}

				if (validUntil != null && (validUntil < DateTime.UtcNow || validUntil < entity.ValidFrom))
				{
					throw new ApiException("ValidUntil must be in the future and be later than the ValidFrom date.", nameof(validUntil), HttpStatusCode.BadRequest);
				}

				entity.ValidUntil = validUntil;
				await UpdateAsync(entity);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// <para>Deactivate an IValidatable Entity based on the ID</para>
		/// <para>if ValidUntil is passed, it needs to be after ValidFrom</para>
		/// </summary>
		/// <param name="id">ID of the entity</param>
		/// <param name="validUntil">optional <see cref="DateTime"/> to determine when the entity is deactive/invalid</param>
		/// <exception cref="ApiException"></exception>
		public async virtual Task DeactivateAsync(TKey id, DateTime? validUntil = null)
		{
			try
			{
				TEntity? entity = await Context.Set<TEntity>().FirstOrDefaultAsync(x => x.ID.Equals(id));

				if (entity == null)
				{
					throw new ApiException($"The provided ID: {id} for type: {typeof(TEntity).Name} doesn't exist in the DB.", nameof(id), HttpStatusCode.BadRequest);
				}

				if (validUntil != null && validUntil < entity.ValidFrom)
				{
					throw new ApiException("ValidUntil can't be before ValidUntil.", nameof(validUntil), HttpStatusCode.BadRequest);
				}

				entity.ValidUntil = validUntil ?? DateTime.Today;
				await UpdateAsync(entity);
			}
			catch
			{
				throw;
			}
		}
	}
}
