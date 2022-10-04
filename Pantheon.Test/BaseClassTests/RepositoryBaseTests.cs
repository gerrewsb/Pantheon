using FluentAssertions;
using Moq;
using Pantheon.Abstractions.Bases;
using Pantheon.Exceptions;
using Pantheon.Test.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pantheon.Test.BaseClassTests
{
	public class RepositoryBaseTests : IClassFixture<TestEntityFixture>
	{
		private readonly TestEntityFixture _fixture;
		private readonly RepositoryBase<DbContextBase, TestEntity, int> _repo;

		public RepositoryBaseTests(TestEntityFixture fixture)
		{
			_fixture = fixture;
			_repo = new(_fixture.DbContext.Object);
		}

		[Fact]
		public async Task GetAllAsync_Should_Return_All_Entities()
		{
			var entities = await _repo.GetAllAsync();
			entities.Should().BeAssignableTo<IEnumerable<TestEntity>>();
			entities.Should().HaveCount(150);
		}

		[Fact]
		public async Task GetAllAsync_Should_Throw_ApiException_If_Result_Is_Empty_List()
		{
			TestEntitySpecBase specs = new(x => x.Count > 100);
			Func<Task> action = async () => await _repo.GetAllAsync(specs);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task GetAllAsync_With_FilterCondition_Should_Return_Filtered_List()
		{
			TestEntitySpecBase specs = new(x => x.ID == 5);
			var result = await _repo.GetAllAsync(specs);
			result.Should().HaveCount(1);
			result.First().ID.Should().Be(5);
		}

		[Fact]
		public async Task GetAllAsync_With_OrderBy_Should_Return_Ordered_List_Ascending()
		{
			TestEntitySpecBase specs = new();
			specs.ApplyOrderBy(x => x.Count);
			var orderedResult = await _repo.GetAllAsync(specs);
			orderedResult.First().Count.Should().BeLessThan(orderedResult.Last().Count);
		}

		[Fact]
		public async Task GetAllAsync_With_OrderByDescending_Should_Return_Ordered_List_Descending()
		{
			TestEntitySpecBase specs = new();
			specs.ApplyOrderByDescending(x => x.Count);
			var orderedResult = await _repo.GetAllAsync(specs);
			orderedResult.First().Count.Should().BeGreaterThan(orderedResult.Last().Count);
		}

		[Fact]
		public async Task GetAllAsync_With_GroupBy_Should_Return_Grouped_List()
		{
			var groupedResult = await _repo.GetAllAsync(x => x.Count, x => new { Key = x.Key, Counter = x.Count() });
			groupedResult.Should().HaveCountLessThan(150);
		}

		[Fact]
		public async Task GetAllAsync_With_GroupBy_Should_Throw_ApiException_If_No_Results()
		{
			TestEntitySpecBase specs = new(x => x.Count > 100);
			Func<Task> action = async () => await _repo.GetAllAsync(x => x.Count, x => new { Key = x.Key, Counter = x.Count() }, specs);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task GetAllAsync_With_Selector_Should_Return_Selected_Result()
		{
			var selectedResult = await _repo.GetAllAsync(x => x.Count);
			selectedResult.Should().BeAssignableTo<IEnumerable<int>>();
		}

		[Fact]
		public async Task GetAllAsync_With_Selector_Should_Throw_ApiException_If_Not_Found()
		{
			TestEntitySpecBase specs = new(x => x.Count > 100);
			Func<Task> action = async () => await _repo.GetAllAsync(x => x.Count, specs);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task GetByIdAsync_Should_Return_Single_Result()
		{
			var result = await _repo.GetByIdAsync(1);
			result.Should().NotBeNull();
			result.Should().BeOfType<TestEntity>();
			result.ID.Should().Be(1);
		}

		[Fact]
		public async Task GetByIdAsync_Should_Throw_ApiException_If_Not_Found()
		{
			Func<Task> action = async () => await _repo.GetByIdAsync(151);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task GetByIdAsync_With_Filter_Should_Return_Result()
		{
			TestEntitySpecBase specs = new(x => x.Count == 40);
			var result = await _repo.GetByIdAsync(6, specs);
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task GetByIdAsync_With_Selector_Throw_ApiException_If_Not_Found()
		{
			TestEntitySpecBase specs = new(x => x.Count > 100);
			Func<Task> action = async () => await _repo.GetByIdAsync(1, x => x.Count, specs);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task GetFirstByExpressionAsync_Should_Return_Result()
		{
			TestEntitySpecBase specs = new(x => x.FirstName == "Brycen" && x.LastName == "White");
			var result = await _repo.GetFirstByExpressionAsync(specs);
			result.Should().NotBeNull();
			result.FirstName.Should().Be("Brycen");
			result.LastName.Should().Be("White");
		}

		[Fact]
		public async Task GetFirstByExpressionAsync_Should_Throw_ApiException_If_Not_Found()
		{
			TestEntitySpecBase specs = new(x => x.FirstName == "Some" && x.LastName == "Dude");
			Func<Task> action = async () => await _repo.GetFirstByExpressionAsync(specs);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task CreateAsync_Should_Insert_One_Entity()
		{
			TestEntity newEntity = new()
			{
				FirstName = "Gert",
				LastName = "Dunon",
				Count = 50
			};

			var createdEntity = await _repo.CreateAsync(newEntity);
			createdEntity.Should().NotBeNull();
			_fixture.DbSet.Verify(m => m.Add(It.IsAny<TestEntity>()), Times.Once());
		}

		[Fact]
		public async Task CreateAsync_Should_Throw_ApiException_If_Entity_Is_Null()
		{
			Func<Task> action = async () => await _repo.CreateAsync(null);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task UpdateAsync_Should_Update_One_Entity()
		{
			var entity = await _repo.GetByIdAsync(100);
			entity.FirstName = "Gert";
			entity.LastName = "Dunon";
			await _repo.UpdateAsync(entity);
			_fixture.DbSet.Verify(m => m.Update(It.Is<TestEntity>(x => x.ID == 100)), Times.Once());
		}

		[Fact]
		public async Task UpdateAsync_Should_Throw_ApiException_If_Entity_Is_Null()
		{
			Func<Task> action = async () => await _repo.UpdateAsync(null);
			await action.Should().ThrowAsync<ApiException>();
		}

		[Fact]
		public async Task DeleteAsync_Should_Delete_One_Entity()
		{
			await _repo.DeleteAsync(50);
			_fixture.DbSet.Verify(m => m.Remove(It.Is<TestEntity>(x => x.ID == 50)), Times.Once());
		}

		[Fact]
		public async Task DeleteAsync_Should_Throw_ApiException_If_Key_Not_Found()
		{
			Func<Task> action = async () => await _repo.DeleteAsync(151);
			await action.Should().ThrowAsync<ApiException>();
		}
	}
}
