// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The user controller class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig.Controllers;

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
    private readonly IMapper autoMapper;

    /// <summary>
    ///     The password hasher.
    /// </summary>
    private readonly IPasswordHasher<User> passwordHasher;

    /// <summary>
    ///     The user repository.
    /// </summary>
    private readonly IUserRepository userRepository;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserController" /> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="autoMapper">The <see cref="IMapper" />.</param>
    /// <param name="userRepository">The <see cref="IUserRepository" />.</param>
    public UserController(ILogger logger, IMapper autoMapper, IUserRepository userRepository)
    {
        this.logger = logger;
        this.autoMapper = autoMapper;
        this.passwordHasher = new PasswordHasher<User>();
        this.userRepository = userRepository;
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
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DtoReadUser>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DtoReadUser>>> GetUsers()
    {
        try
        {
            this.logger.Information("Executed GetUsers.");

            var users = await this.userRepository.GetUsers();
            var usersList = users?.ToList();

            if (usersList?.Count == 0)
            {
                return this.Ok("[]");
            }

            var returnUsers = this.autoMapper.Map<IEnumerable<DtoReadUser>>(users);
            return this.Ok(returnUsers);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DtoReadUser>> GetUserById(Guid userId)
    {
        try
        {
            this.logger.Information("Executed GetUserById with user identifier {@UserId}.", userId);

            var user = await this.userRepository.GetUserById(userId);

            if (user == null)
            {
                this.logger.Warning("User with user identifier {@UserId} not found.", userId);
                return this.NotFound(userId);
            }

            var returnUser = this.autoMapper.Map<DtoReadUser>(user);
            return this.Ok(returnUser);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
    /// <response code="401">Unauthorized.</response>
    /// <response code="409">User already exists.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DtoCreateUpdateUser), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DtoCreateUpdateUser), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateUser([FromBody] DtoCreateUpdateUser createUser)
    {
        try
        {
            this.logger.Information("Executed CreateUser with user {@CreateUser}.", createUser);

            var user = this.autoMapper.Map<User>(createUser);
            user.Id = Guid.NewGuid();

            var userExists = await this.userRepository.UserNameExists(createUser.UserName);

            if (userExists)
            {
                return this.Conflict(createUser);
            }

            var inserted = await this.userRepository.InsertUser(user);

            if (!inserted)
            {
                return this.BadRequest(createUser);
            }

            var returnUser = this.autoMapper.Map<DtoReadUser>(user);
            return this.Ok(returnUser);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(DtoReadUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] DtoCreateUpdateUser updateUser)
    {
        try
        {
            this.logger.Information("Executed UpdateUser with user {@UpdateUser} for user identifier: {@UserId}.", updateUser, userId);

            var resultUser = await this.userRepository.GetUserById(userId);

            if (resultUser == null)
            {
                this.logger.Warning("User with user identifier {@UserId} not found.", userId);
                return this.NotFound(userId);
            }

            resultUser = this.autoMapper.Map<User>(updateUser);
            resultUser.Id = userId;
            resultUser.PasswordHash = this.passwordHasher.HashPassword(resultUser, updateUser.Password);

            var updated = await this.userRepository.UpdateUser(resultUser);

            if (!updated)
            {
                return this.BadRequest(userId);
            }

            var returnUser = this.autoMapper.Map<DtoReadUser>(resultUser);
            return this.Ok(returnUser);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
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
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{userId}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteUserById(Guid userId)
    {
        try
        {
            this.logger.Information("Executed DeleteUserById with user identifier {@UserId}.", userId);
            var deleted = await this.userRepository.DeleteUser(userId);

            if (deleted)
            {
                return this.Ok(userId);
            }

            return this.BadRequest(userId);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
            return this.InternalServerError(ex);
        }
    }
}
