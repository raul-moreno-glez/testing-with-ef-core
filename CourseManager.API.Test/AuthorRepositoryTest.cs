using CourseManager.API.DbContexts;
using CourseManager.API.Entities;
using CourseManager.API.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace CourseManager.API.Test
{
    public class Tests
    {
        [Test]
        public void Given_PageSize_Is_Three_When_GetAuthors_Then_Should_Return_Three_Authors()
        {
            // Given
            // isolate tests using a new database with a different database name
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using var context = new CourseContext(options);
            context.Countries.Add(new Country()
            {
                Id = "MX",
                Description = "Mexico"
            });

            context.Countries.Add(new Country()
            {
                Id = "US",
                Description = "United States of America"
            });

            context.Authors.Add(new Author()
                { FirstName = "Robert", LastName = "Martin", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "Michael", LastName = "Feathers", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "Timothy", LastName = "Ottinger", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "James", LastName = "Grenning", CountryId = "MX" });
            context.Authors.Add(new Author()
                { FirstName = "Kevin", LastName = "Wampler", CountryId = "US" });

            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // When
            var authors = repository.GetAuthors(1, 3);

            // Then
            Assert.AreEqual(3, authors.Count());
        }

        [Test]
        public void Given_PageSize_Is_Three_When_GetAuthors_Then_Should_Return_Three_Authors_v2()
        {
            // Given
            // Using SQLite Provider
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new CourseContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            context.Countries.Add(new Country()
            {
                Id = "MX",
                Description = "Mexico"
            });

            context.Countries.Add(new Country()
            {
                Id = "US",
                Description = "United States of America"
            });

            context.Authors.Add(new Author()
                { FirstName = "Robert", LastName = "Martin", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "Michael", LastName = "Feathers", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "Timothy", LastName = "Ottinger", CountryId = "US" });
            context.Authors.Add(new Author()
                { FirstName = "James", LastName = "Grenning", CountryId = "MX" });
            context.Authors.Add(new Author()
                { FirstName = "Kevin", LastName = "Wampler", CountryId = "US" });

            context.SaveChanges();

            var repository = new AuthorRepository(context);

            // When
            var authors = repository.GetAuthors(1, 3);

            // Then
            Assert.AreEqual(3, authors.Count());
        }

        [Test]
        public void Given_Author_Without_CountryId_When_AddAuthor_Then_Author_Should_Have_MX_As_CountryId()
        {
            // Arrange
            // isolate tests using a new database with a different database name
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new CourseContext(options))
            {
                context.Countries.Add(new Country()
                {
                    Id = "MX",
                    Description = "Mexico"
                });

                context.SaveChanges();
                var authorRepository = new AuthorRepository(context);
                var authorToAdd = new Author()
                {
                    FirstName = "Raul",
                    LastName = "Moreno",
                    Id = Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b")
                };

                // Act
                authorRepository.AddAuthor(authorToAdd);
                authorRepository.SaveChanges();
            }

            using (var context = new CourseContext(options))
            {
                // Assert
                var authorRepository = new AuthorRepository(context);
                var addedAuthor = authorRepository.GetAuthor(
                    Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b"));
                Assert.AreEqual("MX", addedAuthor.CountryId);
            }
        }

    }
}