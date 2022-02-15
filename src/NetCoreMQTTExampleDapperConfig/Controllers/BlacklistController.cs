// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlacklistController.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The blacklist controller class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig.Controllers;

/// <summary>
///     The blacklist controller class.
/// </summary>
/// <seealso cref="ControllerBase" />
[Route("api/blacklist")]
[ApiController]
[OpenApiTag("Blacklist", Description = "Blacklist management.")]
public class BlacklistController : ControllerBase
{
    /// <summary>
    ///     The blacklist repository.
    /// </summary>
    private readonly IBlacklistRepository blacklistRepository;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BlacklistController" /> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="blacklistRepository">The <see cref="IBlacklistRepository" />.</param>
    public BlacklistController(ILogger logger, IBlacklistRepository blacklistRepository)
    {
        this.logger = logger;
        this.blacklistRepository = blacklistRepository;
    }

    /// <summary>
    ///     Gets all blacklist items.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task" /> representing any asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     Gets all blacklist items.
    /// </remarks>
    /// <response code="200">Blacklist items found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BlacklistWhitelist>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BlacklistWhitelist>>> GetAllBlacklistItems()
    {
        try
        {
            this.logger.Information("Executed GetAllBlacklistItems.");
            var blacklistItems = await this.blacklistRepository.GetAllBlacklistItems();
            return this.Ok(blacklistItems);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    ///     Gets a blacklist item by its id.
    /// </summary>
    /// <param name="blacklistItemId">
    ///     The blacklist item identifier.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing any asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     Gets a blacklist item by its id.
    /// </remarks>
    /// <response code="200">Blacklist item found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Blacklist item not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{blacklistItemId}")]
    [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BlacklistWhitelist>> GetBlacklistItemById(Guid blacklistItemId)
    {
        try
        {
            this.logger.Information("Executed GetBlacklistItemById with blacklist item identifier {@BlacklistItemId}.", blacklistItemId);

            var blacklistItem = await this.blacklistRepository.GetBlacklistItemById(blacklistItemId);

            if (blacklistItem != null)
            {
                return this.Ok(blacklistItem);
            }

            this.logger.Warning("Blacklist item with blacklist item identifier {@BlacklistItemId} not found.", blacklistItemId);
            return this.NotFound(blacklistItemId);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    ///     Creates a blacklist item.
    /// </summary>
    /// <param name="createBlacklistItem">
    ///     The create blacklist item.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing any asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     Creates a blacklist item.
    /// </remarks>
    /// <response code="200">Blacklist item created.</response>
    /// <response code="400">Blacklist item not created.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="409">Blacklist item already exists.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateBlacklistItem([FromBody] BlacklistWhitelist createBlacklistItem)
    {
        try
        {
            this.logger.Information("Executed CreateBlacklistItem with blacklist item {@CreateBlacklistItem}.", createBlacklistItem);

            var foundBlackListItem = this.blacklistRepository.GetBlacklistItemByIdAndType(createBlacklistItem.Id, createBlacklistItem.Type);

            if (foundBlackListItem != null)
            {
                return this.Conflict(createBlacklistItem);
            }

            var inserted = await this.blacklistRepository.InsertBlacklistItem(createBlacklistItem);

            if (inserted)
            {
                return this.Ok(createBlacklistItem);
            }

            return this.BadRequest(createBlacklistItem);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
            return this.InternalServerError(ex);
        }
    }

    /// <summary>
    ///     Deletes a blacklist item by its id.
    /// </summary>
    /// <param name="blacklistItemId">
    ///     The blacklist item identifier.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing any asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     Deletes a blacklist item by its id.
    /// </remarks>
    /// <response code="200">Blacklist item deleted.</response>
    /// <response code="400">Blacklist item not deleted.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{blacklistItemId}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteBlacklistItemById(Guid blacklistItemId)
    {
        try
        {
            this.logger.Information("Executed DeleteBlacklistItemById with blacklist item identifier {@BlacklistItemId}.", blacklistItemId);
            var deleted = await this.blacklistRepository.DeleteBlacklistItem(blacklistItemId);

            if (deleted)
            {
                return this.Ok(blacklistItemId);
            }

            return this.BadRequest(blacklistItemId);
        }
        catch (Exception ex)
        {
            this.logger.Fatal("An error occurred: {@Exception}.", ex);
            return this.InternalServerError(ex);
        }
    }
}
