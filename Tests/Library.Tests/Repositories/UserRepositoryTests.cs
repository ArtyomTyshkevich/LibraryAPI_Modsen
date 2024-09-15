using Library.Data.Repositories;
using Library.Tests.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests.Repositories
{
    public class UserRepositoryTests : TestCommandBase
    {
        [Fact]
        public async Task CreateUser_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            var userRepository = new UserRepository(Context);

            // Act
            await userRepository.Create(user, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var createdUser = await Context.Users.FirstOrDefaultAsync(u =>
                u.Id == user.Id &&
                u.UserName == user.UserName &&
                u.Email == user.Email);

            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task GetUserById_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            var result = await userRepository.Get(user.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result!.Id);
            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            await userRepository.Delete(user, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var deletedUser = await Context.Users.FindAsync(user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task UpdateUser_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            user.UserName = "UpdatedUserName";
            await userRepository.Update(user, CancellationToken.None);
            await Context.SaveChangesAsync();

            // Assert
            var updatedUser = await Context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedUserName", updatedUser!.UserName);
        }

        [Fact]
        public async Task GetUsersWithPagination_Success()
        {
            // Arrange
            var user1 = UserTestData.CreateTestUser();
            var user2 = UserTestData.CreateTestUser();
            await Context.Users.AddRangeAsync(user1, user2);
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            var usersPage = await userRepository.GetWithPagination(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(usersPage);
            Assert.Contains(usersPage, u => u.Id == user1.Id || u.Id == user2.Id);
        }

        [Fact]
        public async Task GetUserByMail_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            var result = await userRepository.GetByMail(user.Email, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result!.Email);
        }

        [Fact]
        public async Task GetRoles_Success()
        {
            // Arrange
            var user = UserTestData.CreateTestUser();
            var role = new IdentityRole<long> { Id = 1, Name = "Admin" };
            Context.Users.Add(user);
            Context.Roles.Add(role);
            Context.UserRoles.Add(new IdentityUserRole<long> { UserId = user.Id, RoleId = role.Id });
            await Context.SaveChangesAsync();
            var userRepository = new UserRepository(Context);

            // Act
            var roles = await userRepository.GetRoles(user, CancellationToken.None);

            // Assert
            Assert.NotNull(roles);
            Assert.Contains(roles, r => r.Id == role.Id);
        }
    }
}
