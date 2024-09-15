using Library.Domain.Entities;
using System;

namespace Library.Tests.Common
{
    public static class UserTestData
    {
        public static User CreateTestUser()
        {
            return new User
            {
                Id = new Random().Next(1, 1000), // Генерация случайного Id для теста
                UserName = "TestUser" + Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "M",
                Email = "testuser@example.com",
                RefreshToken = "sampleToken",
                RefreshTokenExpiryTime = DateTime.Now.AddDays(5)
            };
        }
    }
}