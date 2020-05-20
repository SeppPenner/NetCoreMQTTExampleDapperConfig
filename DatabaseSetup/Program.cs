// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   A program to setup the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DatabaseSetup
{
    using System;

    using Microsoft.AspNetCore.Identity;

    using Storage;
    using Storage.Database;
    using Storage.Enumerations;
    using Storage.Repositories.Implementation;
    using Storage.Repositories.Interfaces;

    /// <summary>
    ///     A program to setup the database.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The <see cref="IUserRepository" />.
        /// </summary>
        private static IUserRepository userRepository;

        /// <summary>
        ///     The <see cref="IDatabaseVersionRepository" />.
        /// </summary>
        private static IDatabaseVersionRepository databaseVersionRepository;

        /// <summary>
        ///     The <see cref="IWhitelistRepository" />.
        /// </summary>
        private static IWhitelistRepository whitelistRepository;

        /// <summary>
        ///     The <see cref="IBlacklistRepository" />.
        /// </summary>
        private static IBlacklistRepository blacklistRepository;

        /// <summary>
        ///     The <see cref="IDatabaseHelper" />.
        /// </summary>
        private static IDatabaseHelper databaseHelper;

        /// <summary>
        ///     The main method of the program.
        /// </summary>
        public static void Main()
        {
            var databaseSettings = new DatabaseConnectionSettings
            {
                Host = "localhost",
                Database = "mqtt",
                Port = 5432,
                Username = "postgres",
                Password = "postgres"
            };

            userRepository = new UserRepository(databaseSettings);
            databaseVersionRepository = new DatabaseVersionRepository(databaseSettings);
            whitelistRepository = new WhitelistRepository(databaseSettings);
            blacklistRepository = new BlacklistRepository(databaseSettings);
            databaseHelper = new DatabaseHelper(databaseSettings);

            Console.WriteLine("Delete database...");
            databaseHelper.DeleteDatabase(databaseSettings.Database);

            Console.WriteLine("Create database...");
            databaseHelper.CreateDatabase(databaseSettings.Database);

            Console.WriteLine("Setting up the database tables...");
            databaseHelper.CreateAllTables(true);

            Console.WriteLine("Adding seed data...");
            SeedData();

            Console.WriteLine("Press any key to close this window...");
            Console.ReadKey();
        }

        /// <summary>
        ///     Seeds the database with some data. Use this method to add custom data as needed.
        /// </summary>
        private static void SeedData()
        {
            var version = new DatabaseVersion { Number = 1, Name = "Sicario", CreatedAt = DateTimeOffset.Now };
            databaseVersionRepository.InsertDatabaseVersion(version);

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                UserName = "Hans",
                ClientId = "Hans",
                ValidateClientId = true,
                ThrottleUser = true,
                MonthlyByteLimit = 10000,
                CreatedAt = DateTimeOffset.Now
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Hans");
            userRepository.InsertUser(user);

            // Insert subscription blacklist and whitelist items
            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "a"
            });

            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "b/+"
            });

            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "c/#"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "d"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "e/+"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "f/#"
            });

            // Insert publish blacklist and whitelist items
            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "g"
            });

            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "h/+"
            });

            blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "i/#"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "j"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "k/+"
            });

            whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "l/#"
            });
        }
    }
}
