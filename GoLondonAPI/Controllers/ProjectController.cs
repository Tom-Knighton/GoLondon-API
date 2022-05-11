using System;
using System.ComponentModel.DataAnnotations;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoLondonAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projects;
        private readonly IMetaService _meta;

        public ProjectController(IProjectService projectService, IMetaService meta)
        {
            _projects = projectService;
            _meta = meta;
        }

        [HttpPost("NewProject")]
        [Produces(typeof(Project))]
        public async Task<IActionResult> CreateProject([Required] [FromBody] CreateProjectDTO project)
        {
            Project p = await _projects.CreateProject(project);
            await _meta.SyncLimits();
            return Ok(p);
        }

        [HttpPut("RenameProject/{projectUUID}/{newName}")]
        [Produces(typeof(Project))]
        public async Task<IActionResult> RenameProject(string projectUUID, string newName)
        {
            try
            {
                return Ok(await _projects.RenameProject(projectUUID, newName));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{uuid}")]
        public async Task<IActionResult> DeleteProject(string projectUUID)
        {
            bool success = await _projects.DeleteProject(projectUUID);
            return success ? Ok() : BadRequest("Project deletion failed");
        }

        [HttpGet("User/{userUUID}")]
        [Produces(typeof(List<Project>))]
        public async Task<IActionResult> GetProjectsForUser(string userUUID)
        {
            return Ok(await _projects.GetProjectsForUserAsync(userUUID));
        }
    }
}

