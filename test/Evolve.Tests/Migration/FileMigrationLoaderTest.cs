﻿using System.Linq;
using EvolveDb.Metadata;
using EvolveDb.Migration;
using Xunit;

namespace EvolveDb.Tests.Migration
{
    public class FileMigrationLoaderTest
    {
        [Fact]
        [Category(Test.Migration)]
        public void Load_file_migrations_with_file_loader_works()
        {
            // Arrange
            var loader = new FileMigrationLoader(new EvolveConfiguration { Locations = new[]
            {
                TestContext.Scripts1,
                TestContext.Scripts2,
                TestContext.Scripts1,
                TestContext.Scripts2 + "/PSG"
            }});

            // Act
            var scripts = loader.GetMigrations().ToList();

            // Assert
            Assert.Equal(8, scripts.Count);
            AssertMigration(scripts[0], "1.3.0", "V1_3_0__desc.sql", "desc");
            AssertMigration(scripts[1], "1.3.1", "V1_3_1__desc.sql", "desc");
            AssertMigration(scripts[2], "1.4.0", "V1_4_0__desc.sql", "desc");
            AssertMigration(scripts[3], "1.5.0", "V1_5_0__desc.sql", "desc");
            AssertMigration(scripts[4], "2.0.0", "V2_0_0__desc.sql", "desc");
            AssertMigration(scripts[5], "2.4.0", "V2_4_0__desc.sql", "desc");
            AssertMigration(scripts[6], "3.0.0", "v3_0_0__CI.sql", "CI");
            AssertMigration(scripts[7], "3.0.1", "V3_0_1__CI.Sql", "CI");

            static void AssertMigration(MigrationScript migration, string version, string name, string description)
            {
                Assert.Equal(MetadataType.Migration, migration.Type);
                Assert.Equal(version, migration.Version.Label);
                Assert.Equal(name, migration.Name);
                Assert.Equal(description, migration.Description);
            }
        }

        [Fact]
        [Category(Test.Migration)]
        public void When_duplicate_version_found_with_file_loader_Throws_EvolveException()
        {
            var loader = new FileMigrationLoader(new EvolveConfiguration { Locations = new[] { TestContext.ResourcesFolder } });
            Assert.Throws<EvolveConfigurationException>(() => loader.GetMigrations());
        }

        [Fact]
        [Category(Test.Migration)]
        public void Load_repeatable_file_migrations_with_file_loader_works()
        {
            // Arrange
            var loader = new FileMigrationLoader(new EvolveConfiguration
            {
                Locations = new[]
{
                TestContext.Scripts1,
                TestContext.Scripts2,
                TestContext.Scripts1,
                TestContext.Scripts2 + "/PSG"
            }
            });

            // Act
            var scripts = loader.GetRepeatableMigrations().ToList();

            // Assert
            Assert.Equal(4, scripts.Count);
            AssertMigration(scripts[0], "R__desc_a.sql", "desc a");
            AssertMigration(scripts[1], "R__desc_b.sql", "desc b");
            AssertMigration(scripts[2], "R__desc_c.sql", "desc c");
            AssertMigration(scripts[3], "r__desc_ci.sql", "desc ci");

            static void AssertMigration(MigrationScript migration, string name, string description)
            {
                Assert.Equal(MetadataType.RepeatableMigration, migration.Type);
                Assert.Null(migration.Version);
                Assert.Equal(name, migration.Name);
                Assert.Equal(description, migration.Description);
            }
        }

        [Fact]
        [Category(Test.Migration)]
        public void When_duplicate_name_found_with_file_loader_Throws_EvolveException()
        {
            var loader = new FileMigrationLoader(new EvolveConfiguration { Locations = new[] { TestContext.ResourcesFolder } });
            Assert.Throws<EvolveConfigurationException>(() => loader.GetMigrations());
        }
    }
}
