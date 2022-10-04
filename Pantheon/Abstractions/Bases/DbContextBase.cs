using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pantheon.Abstractions.Contracts;

namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Baseclass for all EFCore based applications
	/// </summary>
	public class DbContextBase : DbContext
	{
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="options"></param>
		public DbContextBase(DbContextOptions options)
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

				if (currentEntity is ITrackableEntity trackableEntity)
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
