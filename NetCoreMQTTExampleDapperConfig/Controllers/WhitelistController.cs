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

namespace NetCoreMQTTExampleDapperConfig.Controllers
{
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
        /// The whitelist repository.
        /// </summary>
        private readonly IWhitelistRepository _whitelistRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WhitelistController" /> class.
        /// </summary>
        /// <param name="whitelistRepository">The <see cref="IWhitelistRepository"/>.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public WhitelistController(IWhitelistRepository whitelistRepository)
        {
            _whitelistRepository = whitelistRepository;
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
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlacklistWhitelist>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlacklistWhitelist>>> GetAllWhitelistItems()
        {
            try
            {
                Log.Information("Executed GetAllWhitelistItems().");
                var whitelistItems = await _whitelistRepository.GetAllWhitelistItems();
                return Ok(whitelistItems);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="404">Whitelist item not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{whitelistItemId}")]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlacklistWhitelist>> GetWhitelistItemById(Guid whitelistItemId)
        {
            try
            {
                Log.Information($"Executed GetWhitelistItemById({whitelistItemId}).");

                var whitelistItem = await _whitelistRepository.GetWhitelistItemById(whitelistItemId);

                if (whitelistItem != null)
                {
                    return Ok(whitelistItem);
                }

                Log.Warning($"Whitelist item with identifier {whitelistItemId} not found.");
                return NotFound(whitelistItemId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="409">Whitelist item already exists.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateWhitelistItem([FromBody] BlacklistWhitelist createWhitelistItem)
        {
            try
            {
                Log.Information($"Executed CreateWhitelistItem({createWhitelistItem}).");

                var foundBlackListItem = _whitelistRepository.GetWhitelistItemByIdAndType(createWhitelistItem.Id, createWhitelistItem.Type);

                if (foundBlackListItem != null)
                {
                    return Conflict("Whitelist item already exists.");
                }

                var inserted = await _whitelistRepository.InsertWhitelistItem(createWhitelistItem);
                if (inserted) return Ok(createWhitelistItem);
                return BadRequest(createWhitelistItem);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{whitelistItemId}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteWhitelistItemById(Guid whitelistItemId)
        {
            try
            {
                Log.Information($"Executed DeleteWhitelistItemById({whitelistItemId}).");
                var deleted = await _whitelistRepository.DeleteWhitelistItem(whitelistItemId);
                if (deleted) return Ok(whitelistItemId);
                return BadRequest(whitelistItemId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}