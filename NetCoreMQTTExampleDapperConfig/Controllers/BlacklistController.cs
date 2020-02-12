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
    ///     The blacklist controller class.
    /// </summary>
    /// <seealso cref="ControllerBase" />
    [Route("api/blacklist")]
    [ApiController]
    [OpenApiTag("Blacklist", Description = "Blacklist management.")]
    public class BlacklistController : ControllerBase
    {
        /// <summary>
        /// The blacklist repository.
        /// </summary>
        private readonly IBlacklistRepository _blacklistRepository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlacklistController" /> class.
        /// </summary>
        /// <param name="blacklistRepository">The <see cref="IBlacklistRepository"/>.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public BlacklistController(IBlacklistRepository blacklistRepository)
        {
            _blacklistRepository = blacklistRepository;
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
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BlacklistWhitelist>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BlacklistWhitelist>>> GetAllBlacklistItems()
        {
            try
            {
                Log.Information("Executed GetAllBlacklistItems().");
                var blacklistItems = await _blacklistRepository.GetAllBlacklistItems();
                return Ok(blacklistItems);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="404">Blacklist item not found.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpGet("{blacklistItemId}")]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlacklistWhitelist>> GetBlacklistItemById(Guid blacklistItemId)
        {
            try
            {
                Log.Information($"Executed GetBlacklistItemById({blacklistItemId}).");

                var blacklistItem = await _blacklistRepository.GetBlacklistItemById(blacklistItemId);

                if (blacklistItem != null)
                {
                    return Ok(blacklistItem);
                }

                Log.Warning($"Blacklist item with identifier {blacklistItemId} not found.");
                return NotFound(blacklistItemId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="409">Blacklist item already exists.</response>
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpPost]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BlacklistWhitelist), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateBlacklistItem([FromBody] BlacklistWhitelist createBlacklistItem)
        {
            try
            {
                Log.Information($"Executed CreateBlacklistItem({createBlacklistItem}).");

                var foundBlackListItem = _blacklistRepository.GetBlacklistItemByIdAndType(createBlacklistItem.Id, createBlacklistItem.Type);

                if (foundBlackListItem != null)
                {
                    return Conflict(createBlacklistItem);
                }

                var inserted = await _blacklistRepository.InsertBlacklistItem(createBlacklistItem);
                if (inserted) return Ok(createBlacklistItem);
                return BadRequest(createBlacklistItem);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
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
        /// <response code="500">Internal server error.</response>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [HttpDelete("{blacklistItemId}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBlacklistItemById(Guid blacklistItemId)
        {
            try
            {
                Log.Information($"Executed DeleteBlacklistItemById({blacklistItemId}).");
                var deleted = await _blacklistRepository.DeleteBlacklistItem(blacklistItemId);
                if (deleted) return Ok(blacklistItemId);
                return BadRequest(blacklistItemId);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message, ex);
                return this.InternalServerError(ex);
            }
        }
    }
}