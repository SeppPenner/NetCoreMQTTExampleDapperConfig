using System;
using Microsoft.AspNetCore.Identity;
using Storage;
using Storage.Database;
using Storage.Enumerations;
using Storage.Repositories.Implementation;
using Storage.Repositories.Interfaces;

namespace DatabaseSetup
{
    /// <summary>
    ///     A program to setup the database.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The <see cref="IUserRepository"/>.
        /// </summary>
        private static IUserRepository _userRepository;

        /// <summary>
        /// The <see cref="IDatabaseVersionRepository"/>.
        /// </summary>
        private static IDatabaseVersionRepository _databaseVersionRepository;

        /// <summary>
        /// The <see cref="IWhitelistRepository"/>.
        /// </summary>
        private static IWhitelistRepository _whitelistRepository;

        /// <summary>
        /// The <see cref="IBlacklistRepository"/>.
        /// </summary>
        private static IBlacklistRepository _blacklistRepository;

        /// <summary>
        /// The <see cref="IDatabaseHelper"/>.
        /// </summary>
        private static IDatabaseHelper _databaseHelper;

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

            _userRepository = new UserRepository(databaseSettings);
            _databaseVersionRepository = new DatabaseVersionRepository(databaseSettings);
            _whitelistRepository = new WhitelistRepository(databaseSettings);
            _blacklistRepository = new BlacklistRepository(databaseSettings);
            _databaseHelper = new DatabaseHelper(databaseSettings);

            Console.WriteLine("Delete database...");
            _databaseHelper.DeleteDatabase(databaseSettings.Database);

            Console.WriteLine("Create database...");
            _databaseHelper.CreateDatabase(databaseSettings.Database);

            Console.WriteLine("Setting up the database tables...");
            _databaseHelper.CreateAllTables(true);

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
            _databaseVersionRepository.InsertDatabaseVersion(version);

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
            _userRepository.InsertUser(user);

            // Insert subscription blacklist and whitelist items
            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "a"
            });

            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "b/+"
            });

            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "c/#"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "d"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "e/+"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Subscribe,
                Value = "f/#"
            });

            // Insert publish blacklist and whitelist items
            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "g"
            });

            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "h/+"
            });

            _blacklistRepository.InsertBlacklistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "i/#"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "j"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "k/+"
            });

            _whitelistRepository.InsertWhitelistItem(new BlacklistWhitelist
            {
                UserId = userId,
                Type = BlacklistWhitelistType.Publish,
                Value = "l/#"
            });
        }
    }
}