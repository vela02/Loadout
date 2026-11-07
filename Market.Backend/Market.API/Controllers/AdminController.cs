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
    private readonly Market.Infrastructure.BackgroundJobs.BackgroundJobMonitor _jobMonitor;
    private readonly Market.Infrastructure.BackgroundJobs.ScheduledTasksService _scheduler;

    public AdminController(
        IAdminService adminService,
        Market.Infrastructure.BackgroundJobs.BackgroundJobMonitor jobMonitor,
        Market.Infrastructure.BackgroundJobs.ScheduledTasksService scheduler)
    {
        _adminService = adminService;
        _jobMonitor = jobMonitor;
        _scheduler = scheduler;
    }

    // ---------- POZADINSKI PROCESI ----------

    // GET api/Admin/jobs -> stanje svih zadataka + historija zadnjih 50 izvršavanja
    [HttpGet("jobs")]
    public IActionResult GetJobs()
    {
        return Ok(_jobMonitor.GetSnapshot());
    }

    // POST api/Admin/jobs/ReleasePreOrders/run -> ručno pokretanje ("Pokreni sada")
    [HttpPost("jobs/{name}/run")]
    public async Task<IActionResult> RunJob(string name)
    {
        if (!_jobMonitor.Exists(name)) return NotFound(new { message = "Zadatak ne postoji." });
        await _scheduler.RunJobAsync(name, "Manual");
        return Ok(_jobMonitor.GetSnapshot());
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

    

    
    // GET api/Admin/announcements?title=&type=&isActive=&createdFrom=&createdTo=&page=&pageSize=
    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements([FromQuery] AnnouncementFilterDto filter)
    {
        return Ok(await _adminService.GetAnnouncementsPagedAsync(filter));
    }

    // PUT api/Admin/announcements/5 -> izmjena (ista validacija kao kod kreiranja)
    [HttpPut("announcements/{id}")]
    public async Task<IActionResult> UpdateAnnouncement(int id, CreateNotificationDto dto)
    {
        var ok = await _adminService.UpdateAnnouncementAsync(id, dto);
        return ok ? NoContent() : NotFound(new { message = "Obavještenje nije pronađeno." });
    }

    // DELETE api/Admin/announcements/5
    [HttpDelete("announcements/{id}")]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var ok = await _adminService.DeleteAnnouncementAsync(id);
        return ok ? NoContent() : NotFound(new { message = "Obavještenje nije pronađeno." });
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