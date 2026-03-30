using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using friendzone_backend.Data;
using friendzone_backend.DTOs;
using friendzone_backend.Entities;
using friendzone_backend.Services;

namespace friendzone_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InviteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CurrentUserService _currentUser;

        public InviteController(AppDbContext context, CurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // Davet gönder
        [HttpPost("send")]
        public async Task<IActionResult> Send(SendInviteDto dto)
        {
            var senderId = _currentUser.UserId;

            if (senderId == dto.ReceiverId)
                return BadRequest("Kendine davet gönderemezsin");

            // Zaten bekleyen davet var mı?
            var exists = await _context.Invites.AnyAsync(x =>
                x.SenderId == senderId &&
                x.ReceiverId == dto.ReceiverId &&
                x.Status == InviteStatus.Pending);

            if (exists)
                return BadRequest("Zaten bekleyen bir davet var");

            var invite = new Invite
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Status = InviteStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invites.Add(invite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Davet gönderildi", inviteId = invite.Id });
        }

        // Gelen davetleri listele
        [HttpGet("incoming")]
        public async Task<IActionResult> Incoming()
        {
            var userId = _currentUser.UserId;

            var invites = await _context.Invites
                .Include(x => x.Sender)
                .Where(x => x.ReceiverId == userId && x.Status == InviteStatus.Pending)
                .Select(x => new
                {
                    x.Id,
                    SenderUsername = x.Sender.Username,
                    x.CreatedAt
                })
                .ToListAsync();

            return Ok(invites);
        }

        // Daveti kabul et / reddet
        [HttpPost("respond")]
        public async Task<IActionResult> Respond(InviteResponseDto dto)
        {
            var userId = _currentUser.UserId;

            var invite = await _context.Invites
                .FirstOrDefaultAsync(x => x.Id == dto.InviteId && x.ReceiverId == userId);

            if (invite == null)
                return NotFound("Davet bulunamadı");

            if (invite.Status != InviteStatus.Pending)
                return BadRequest("Bu davet zaten yanıtlanmış");

            invite.Status = dto.Accept ? InviteStatus.Accepted : InviteStatus.Rejected;
            invite.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var message = dto.Accept ? "Davet kabul edildi" : "Davet reddedildi";
            return Ok(new { message });
        }
    }
}