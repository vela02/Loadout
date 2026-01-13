using Market.Application.Abstractions;
using Market.Domain.Models;
using Market.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Market.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]// Svi endpointi u ovom kontroleru zahtijevaju ulogu "Admin"
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }


    
    [HttpGet("comments")]
    public async Task<ActionResult<List<CommentModerationDto>>> GetComments()
    {
        var comments = await _adminService.GetAllCommentsAsync();
        return Ok(comments);
    }

    
    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var success = await _adminService.DeleteCommentAsync(id);

        if (!success)
            return NotFound("Komentar nije pronađen.");

        return Ok("Komentar je uspješno uklonjen.");
    }

    

    
    [HttpPost("announcements")]
    public async Task<IActionResult> PostAnnouncement(CreateNotificationDto dto)
    {
        var success = await _adminService.CreateAnnouncementAsync(dto);

        if (!success)
            return BadRequest("Došlo je do greške prilikom objave obavještenja.");

        return Ok("Obavještenje je uspješno objavljeno svim korisnicima.");
    }

    [HttpPut("announcements/{id}/toggle")]
    public async Task<IActionResult> ToggleAnnouncement(int id)
    {
        var success = await _adminService.ToggleNotificationStatusAsync(id);
        if (!success) return NotFound("Obavještenje nije pronađeno.");

        return Ok("Status obavještenja je uspješno promijenjen.");
    }
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs()
    {
        return Ok(await _adminService.GetAuditLogsAsync());
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashBoardStatsDto>> GetStats()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return Ok(stats);
    }

}