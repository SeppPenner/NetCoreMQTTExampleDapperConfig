// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhitelistController.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The whitelist controller class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleDapperConfig.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using NetCoreMQTTExampleDapperConfig.Controllers.Extensions;

    using NSwag.Annotations;

    using Serilog;

    using Storage.Database;
    using Storage.Repositories.Interfaces;

    /// <summary>
    ///     The whitelist controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/whitelist")]
    [ApiController]
    [OpenApiTag("Whitelist", Description = "Whitelist management.")]
    public class WhitelistController : ControllerBase
    {
        /// <summary>
        ///     The whitelist repository.
        /// </summary>
        private readonly IWhitelistRepository whitelistRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger = Log.ForContext<WhitelistController>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="WhitelistController" /> class.
        /// </summary>
        /// <param name="whitelistRepository">The <see cref="IWhitelistRepository" />.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public WhitelistController(IWhitelistRepository whitelistRepository)
        {
            this.whitelistRepository = whitelistRepository;
        }

        /// <summary>
        ///     Gets all whitelist items.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets all whitelist items.
        /// </remarks>
        /// <response code="200">Whitelist items found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlacklistWhitelist>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlacklistWhitelist>>> GetAllWhitelistItems()
        {
            try
            {
                this.logger.Information("Executed GetAllWhitelistItems.");
                var whitelistItems = await this.whitelistRepository.GetAllWhitelistItems();
                return this.Ok(whitelistItems);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Gets a whitelist item by its id.
        /// </summary>
        /// <param name="whitelistItemId">
        ///     The whitelist item identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Gets a whitelist item by its id.
        /// </remarks>
        /// <response code="200">Whitelist item found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Whitelist item not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{whitelistItemId}")]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlacklistWhitelist>> GetWhitelistItemById(Guid whitelistItemId)
        {
            try
            {
                this.logger.Information("Executed GetWhitelistItemById with whitelist item identifier {@WhitelistItemId}.", whitelistItemId);

                var whitelistItem = await this.whitelistRepository.GetWhitelistItemById(whitelistItemId);

                if (whitelistItem != null)
                {
                    return this.Ok(whitelistItem);
                }

                this.logger.Warning("Whitelist item with identifier {@WhitelistItemId} not found.", whitelistItemId);
                return this.NotFound(whitelistItemId);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Creates a whitelist item.
        /// </summary>
        /// <param name="createWhitelistItem">
        ///     The create whitelist item.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Creates a whitelist item.
        /// </remarks>
        /// <response code="200">Whitelist item created.</response>
        /// <response code="400">Whitelist item not created.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="409">Whitelist item already exists.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateWhitelistItem([FromBody] BlacklistWhitelist createWhitelistItem)
        {
            try
            {
                this.logger.Information("Executed CreateWhitelistItem with whitelist item {@CreateWhitelistItem}.", createWhitelistItem);

                var foundBlackListItem = this.whitelistRepository.GetWhitelistItemByIdAndType(createWhitelistItem.Id, createWhitelistItem.Type);

                if (foundBlackListItem != null)
                {
                    return this.Conflict(createWhitelistItem);
                }

                var inserted = await this.whitelistRepository.InsertWhitelistItem(createWhitelistItem);

                if (inserted)
                {
                    return this.Ok(createWhitelistItem);
                }

                return this.BadRequest(createWhitelistItem);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
                return this.InternalServerError(ex);
            }
        }

        /// <summary>
        ///     Deletes a whitelist item by its id.
        /// </summary>
        /// <param name="whitelistItemId">
        ///     The whitelist item identifier.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> representing any asynchronous operation.
        /// </returns>
        /// <remarks>
        ///     Deletes a whitelist item by its id.
        /// </remarks>
        /// <response code="200">Whitelist item deleted.</response>
        /// <response code="400">Whitelist item not deleted.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1625:ElementDocumentationMustNotBeCopiedAndPasted", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{whitelistItemId}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteWhitelistItemById(Guid whitelistItemId)
        {
            try
            {
                this.logger.Information("Executed DeleteWhitelistItemById with whitelist item identifier {@WhitelistItemId}.", whitelistItemId);
                var deleted = await this.whitelistRepository.DeleteWhitelistItem(whitelistItemId);

                if (deleted)
                {
                    return this.Ok(whitelistItemId);
                }

                return this.BadRequest(whitelistItemId);
            }
            catch (Exception ex)
            {
                this.logger.Fatal("An error occurred: {@Exception}.", ex);
                return this.InternalServerError(ex);
            }
        }
    }
}
