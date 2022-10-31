using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using Pantheon.EntityFrameworkCore.Abstractions.Bases;
using Pantheon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
namespace Pantheon.Test.Fixtures
{
	public class TestEntityFixture : IDisposable
	{
		internal Mock<DbSet<TestEntity>> DbSet { get; init; }
		internal Mock<DbContextBase> DbContext { get; init; }

		public TestEntityFixture()
		{
			var options = new DbContextOptionsBuilder().Options;
			List<TestEntity> entities = new();

			int id = 1;
			Faker<TestEntity> faker = new Faker<TestEntity>()
				.UseSeed(911121)
				.StrictMode(false)
				.RuleFor(x => x.ID, f => id++)
				.RuleFor(x => x.FirstName, f => f.Name.FirstName())
				.RuleFor(x => x.LastName, f => f.Name.LastName())
				.RuleFor(x => x.Count, f => f.Random.Number(0, 100));

			for (int i = 0; i < 150; i++)
			{
				entities.Add(faker.Generate());
			}

			DbSet = entities.AsQueryable().ToAsyncDbSetMock();
			DbContext = new Mock<DbContextBase>(MockBehavior.Loose, options);
			DbContext.Setup(x => x.Set<TestEntity>()).Returns(DbSet.Object);
			DbContext.Setup(x => x.Set<TestEntity>().AsQueryable()).Returns(DbSet.Object.AsQueryable());
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}

	internal class TestEntity : EntityBase<int>
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public int Count { get; set; }
	}
}
