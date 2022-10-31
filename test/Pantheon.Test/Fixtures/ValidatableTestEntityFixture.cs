using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pantheon.Abstractions.Contracts;
using Pantheon.EntityFrameworkCore.Abstractions.Bases;
using Pantheon.Extensions;
using Pantheon.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pantheon.Test.Fixtures
{
	public class ValidatableTestEntityFixture : IDisposable
	{
		internal Mock<DbSet<ValidatableTestEntity>> DbSet { get; init; }
		internal Mock<DbContextBase> DbContext { get; init; }

		public ValidatableTestEntityFixture()
		{
			var options = new DbContextOptionsBuilder().Options;
			List<ValidatableTestEntity> entities = new();

			int id = 1;
			Faker<ValidatableTestEntity> validEntityFaker = new Faker<ValidatableTestEntity>()
				.UseSeed(911121)
				.RuleFor(x => x.ID, f => id++)
				.RuleFor(x => x.FirstName, f => f.Name.FirstName())
				.RuleFor(x => x.LastName, f => f.Name.LastName())
				.RuleFor(x => x.Count, f => f.Random.Number(0, 100))
				.RuleFor(x => x.ValidFrom, f => f.Date.Between(new(3000, 1, 1), new(3000, 12, 31)))
				.RuleFor(x => x.ValidUntil, (f, entity) => f.Date.Between(entity.ValidFrom!.Value.AddYears(1), entity.ValidFrom!.Value.AddYears(10)).OrNull(f, .5f));

			Faker<ValidatableTestEntity> invalidEntityFaker = new Faker<ValidatableTestEntity>()
				.UseSeed(911121)
				.RuleFor(x => x.ID, f => id++)
				.RuleFor(x => x.FirstName, f => f.Name.FirstName())
				.RuleFor(x => x.LastName, f => f.Name.LastName())
				.RuleFor(x => x.Count, f => f.Random.Number(0, 100))
				.RuleFor(x => x.ValidFrom, f => f.Date.Between(new(3000, 1, 1), new(3000, DateTime.Today.Month, DateTime.Today.Day)))
				.RuleFor(x => x.ValidUntil, (f, entity) => entity.ValidFrom!.Value.AddDays(-1));

			for (int i = 0; i < 75; i++)
			{
				entities.Add(validEntityFaker.Generate());
				entities.Add(invalidEntityFaker.Generate());
			}

			DbSet = entities.AsQueryable().ToAsyncDbSetMock();
			DbContext = new Mock<DbContextBase>(MockBehavior.Loose, options);
			DbContext.Setup(x => x.Set<ValidatableTestEntity>()).Returns(DbSet.Object);
			DbContext.Setup(x => x.Set<ValidatableTestEntity>().AsQueryable()).Returns(new TestAsyncEnumerable<ValidatableTestEntity>(entities));
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}

	internal class ValidatableTestEntity : EntityBase<int>, IValidatable
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public int Count { get; set; }
		public DateTime? ValidFrom { get; set; }
		public DateTime? ValidUntil { get; set; }
	}
}
