using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreMQTTExampleDapperConfig.Controllers.Extensions;
using NSwag.Annotations;
using Serilog;
using Storage.Database;
using Storage.Dto;
using Storage.Repositories.Interfaces;

namespace NetCoreMQTTExampleDapperConfig.Controllers
{
    /// <summary>
    ///     The user controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/user")]
    [ApiController]
    [OpenApiTag("User", Description = "User management.")]
    public class UserController : ControllerBase
    {
        /// <summary>
        ///     The auto mapper.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private readonly IMapper _autoMapper;

        /// <summary>
        ///     The password hasher.
        /// </summary>
        private readonly PasswordHasher<User> _passwordHasher;

        /// <summary>
        ///     The user repository.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="autoMapper">The <see cref="IMapper" />.</param>
        /// <param name="userRepository">The <see cref="IUserRepository" />.</param>
        // ReSharper disable once StyleCop.SA1650
        public UserController(IMapper autoMapper, IUserRepository userRepository)
        {
            _autoMapper = autoMapper;
            _passwordHasher = new PasswordHasher<User>();
            _userRepository = userRepository;
        }

        /// <summary>
        ///     Gets all users.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets all users.
        /// </remarks>
        /// <response code="200">Users found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DtoReadUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DtoReadUser>>> GetUsers()
        {
            try
            {
                Log.Information("Executed GetUsers().");

                var users = await _userRepository.GetUsers();
                var usersList = users?.ToList();

                if (usersList?.Count == 0)
                {
                    return Ok("[]");
                }

                var returnUsers = _autoMapper.Map<IEnumerable<DtoReadUser>>(users);
                return Ok(returnUsers);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Gets a user by their id.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets a user by their id.
        /// </remarks>
        /// <response code="200">User found.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DtoReadUser>> GetUserById(Guid userId)
        {
            try
            {
                Log.Information($"Executed GetUserById({userId}).");

                var user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return NotFound(userId);
                }

                var returnUser = _autoMapper.Map<DtoReadUser>(user);
                return Ok(returnUser);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Creates a user.
        /// </summary>
        /// <param name="createUser">
        ///     The create user.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Creates a user.
        /// </remarks>
        /// <response code="200">User created.</response>
        /// <response code="400">User not created.</response>
        /// <response code="409">User already exists.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DtoCreateUpdateUser), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DtoCreateUpdateUser), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateUser([FromBody] DtoCreateUpdateUser createUser)
        {
            try
            {
                Log.Information($"Executed CreateUser({createUser}).");

                var user = _autoMapper.Map<User>(createUser);
                user.Id = Guid.NewGuid();

                var userExists = await _userRepository.UserNameExists(createUser.UserName);

                if (userExists)
                {
                    return Conflict(createUser);
                }

                var inserted = await _userRepository.InsertUser(user);

                if (!inserted)
                {
                    return BadRequest(createUser);
                }

                var returnUser = _autoMapper.Map<DtoReadUser>(user);
                return Ok(returnUser);

            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Updates a user.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <param name="updateUser">
        ///     The update user.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Updates a user.
        /// </remarks>
        /// <response code="200">User updated.</response>
        /// <response code="400">User not updated.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] DtoCreateUpdateUser updateUser)
        {
            try
            {
                Log.Information($"Executed UpdateUser({updateUser}) for user identifier: {userId}.");

                var resultUser = await _userRepository.GetUserById(userId);

                if (resultUser == null)
                {
                    Log.Warning($"User with identifier {userId} not found.");
                    return NotFound(userId);
                }

                resultUser = _autoMapper.Map<User>(updateUser);
                resultUser.Id = userId;
                resultUser.PasswordHash = _passwordHasher.HashPassword(resultUser, updateUser.Password);

                var updated = await _userRepository.UpdateUser(resultUser);

                if (!updated)
                {
                    return BadRequest(userId);
                }

                var returnUser = _autoMapper.Map<DtoReadUser>(resultUser);
                return Ok(returnUser);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Deletes the user by their id.
        /// </summary>
        /// <param name="userId">
        ///     The user identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Deletes a user by their id.
        /// </remarks>
        /// <response code="200">User deleted.</response>
        /// <response code="400">User not deleted.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserById(Guid userId)
        {
            try
            {
                Log.Information($"Executed DeleteUserById({userId}).");
                var deleted = await _userRepository.DeleteUser(userId);

                if (deleted)
                {
                    return Ok(userId);
                }

                return BadRequest(userId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}
