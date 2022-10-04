using FluentAssertions;
using Moq;
using Pantheon.Abstractions.Bases;
using Pantheon.Exceptions;
using Pantheon.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pantheon.Test.BaseClassTests
{
	public class ValidatableRepositoryBaseTests : IClassFixture<ValidatableTestEntityFixture>
	{
		private readonly ValidatableTestEntityFixture _fixture;
		private readonly ValidatableRepositoryBase<DbContextBase, ValidatableTestEntity, int> _repo;

		public ValidatableRepositoryBaseTests(ValidatableTestEntityFixture fixture)
		{
			_fixture = fixture;
			_repo = new(_fixture.DbContext.Object);
		}

		[Fact]
		public async Task ActivateAsync_Should_Activate_Entity()
		{
			await _repo.ActivateAsync(80);
			_fixture.DbSet.Verify(m => m.Update(It.Is<ValidatableTestEntity>(x => x.ID == 80 && x.ValidUntil == null)), Times.Once());
		}

		[Fact]
		public async Task ActivateAsync_With_ValidUntil_Parameter_Should_Set_ValidUntil_To_Specified_Date()
		{
			DateTime validUntil = new(3002, 1, 1);
			await _repo.ActivateAsync(81, validUntil);
			_fixture.DbSet.Verify(m => m.Update(It.Is<ValidatableTestEntity>(x => x.ID == 81 && x.ValidUntil == validUntil)), Times.Once());
		}

		[Fact]
		public async Task ActivateAsync_With_ValidUntil_In_The_Past_Should_Throw_ApiException()
		{
			//UtcNow will be in the past until the year 3000
			DateTime validUntil = DateTime.UtcNow;
			Func<Task> action = async () => await _repo.ActivateAsync(82, validUntil);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task ActivateAsync_With_ValidUntil_Before_ValidFrom_Should_Throw_ApiException()
		{
			//the object to test against has a ValidFrom value of: 22/11/3000 14:31:08
			DateTime validUntil = new(2999, 11, 21);
			Func<Task> action = async () => await _repo.ActivateAsync(1, validUntil);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task ActivateAsync_Should_Throw_ApiException_If_Entity_Is_Not_Found()
		{
			Func<Task> action = async () => await _repo.ActivateAsync(200);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task DeactivateAsync_Should_Deactivate_Entity()
		{
			await _repo.DeactivateAsync(2);
			_fixture.DbSet.Verify(m => m.Update(It.Is<ValidatableTestEntity>(x => x.ID == 2 && x.ValidUntil == DateTime.Today)), Times.Once());
		}

		[Fact]
		public async Task DeactivateAsync_With_ValidUntil_Parameter_Should_Set_ValidUntil_To_Specified_Date()
		{
			DateTime validUntil = new(3001, 1, 1);
			await _repo.DeactivateAsync(3, validUntil);
			_fixture.DbSet.Verify(m => m.Update(It.Is<ValidatableTestEntity>(x => x.ID == 3 && x.ValidUntil == validUntil)), Times.Once());
		}

		[Fact]
		public async Task DeactivateAsync_With_ValidUntil_Parameter_Before_ValidFrom_Should_Throw_ApiException()
		{
			//TestObject's ValidFrom is: 26/05/3000 10:24:22
			DateTime validUntil = new(3000, 1, 1);
			Func<Task> action = async () => await _repo.DeactivateAsync(5, validUntil);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task DeactivateAsync_Should_Throw_ApiException_If_Entity_Is_Not_Found()
		{
			Func<Task> action = async () => await _repo.DeactivateAsync(200);
			await action.Should().ThrowAsync<ApiException>();
		}
	}
}
