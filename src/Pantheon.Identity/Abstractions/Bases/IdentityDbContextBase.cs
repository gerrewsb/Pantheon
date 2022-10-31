using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pantheon.Abstractions.Contracts;

namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Baseclass for EFCore based applications that implement ASP.NET Core Identity
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	/// <typeparam name="TRole"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TUserClaim"></typeparam>
	/// <typeparam name="TUserRole"></typeparam>
	/// <typeparam name="TUserLogin"></typeparam>
	/// <typeparam name="TRoleClaim"></typeparam>
	/// <typeparam name="TUserToken"></typeparam>
	public class IdentityDbContextBase<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
		where TUser : IdentityUser<TKey>
		where TRole : IdentityRole<TKey>
		where TKey : IEquatable<TKey>
		where TUserClaim : IdentityUserClaim<TKey>
		where TUserRole : IdentityUserRole<TKey>
		where TUserLogin : IdentityUserLogin<TKey>
		where TRoleClaim : IdentityRoleClaim<TKey>
		where TUserToken : IdentityUserToken<TKey>
	{
		public IdentityDbContextBase(DbContextOptions options)
			: base(options)
		{
			SavingChanges += OnSavingChanges;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				throw new InvalidOperationException("OptionsBuilder must be configured!");
			}

			base.OnConfiguring(optionsBuilder);
		}

		/// <summary>
		/// method that's used in EFCore's SavingChanges event.
		/// It sets the LastUpdate of an ITrackableEntity and the ValidFrom property of an IValidatable if the ValidFrom property has a default value.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnSavingChanges(object? sender, SavingChangesEventArgs e)
		{
			foreach (EntityEntry entry in ChangeTracker.Entries())
			{
				var currentEntity = entry.Entity;

				if (currentEntity is ITrackable trackableEntity)
				{
					trackableEntity.LastUpdate = DateTime.UtcNow;
				}

				if (currentEntity is IValidatable validatableEntity && validatableEntity.ValidFrom == default)
				{
					validatableEntity.ValidFrom = DateTime.UtcNow;
				}
			}
		}
	}
}
