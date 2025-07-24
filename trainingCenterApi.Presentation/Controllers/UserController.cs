using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Domain.Models;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Common.Exceptions;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ICurrentUserService currentUser;

    public UsersController(IUserService userService, ICurrentUserService currentUser)
    {
        this.userService = userService;
        this.currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        var existingUsers = await userService.RetrieveAllUsersAsync();
        if (existingUsers.Any(u => u.Username == userDto.Username && u.TenantId == userDto.TenantId))
            throw new AlreadyExistsException("Bu username allaqachon mavjud");

        if (userDto.Role == Role.SuperAdmin)
            return Forbid("SuperAdmin foydalanuvchisini yaratish mumkin emas");

        if (currentUser.Role == "Admin")
        {
            var allowedRoles = new[] { Role.Teacher, Role.Secretary, Role.Student };

            if (!allowedRoles.Contains(userDto.Role))
                return Forbid("Admin faqat Teacher, Secretary yoki Student foydalanuvchisini yaratishi mumkin");

            userDto.TenantId = currentUser.TenantId;
        }

        if (currentUser.Role == "SuperAdmin")
        {
            if (userDto.Role != Role.Admin)
                return Forbid("SuperAdmin faqat Admin foydalanuvchisini yaratishi mumkin");

            if (userDto.TenantId == Guid.Empty)
                return BadRequest("TenantId bo‘sh bo‘lmasligi kerak");
        }

        var resultDto = await userService.CreateUserAsync(userDto);
        return CreatedAtAction(nameof(GetUserById), new { id = resultDto.Id }, resultDto);
    }



    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var allUsers = await userService.RetrieveAllUsersAsync();
        var filtered = currentUser.Role == "SuperAdmin"
            ? allUsers
            : allUsers.Where(u => u.TenantId == currentUser.TenantId).ToList();

        var paged = filtered.Skip((page - 1) * size).Take(size).ToList();

        return Ok(new PagedResult<UserDto>
        {
            Items = paged,
            TotalCount = filtered.Count,
            PageNumber = page,
            PageSize = size
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var userDto = await userService.RetrieveUserByIdAsync(id);

        if (currentUser.Role == "Admin "&& userDto.TenantId != currentUser.TenantId)
            return Forbid();

        return Ok(userDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userDto)
    {
        if (id != userDto.Id) return BadRequest("ID mos kelmadi");

        var existing = await userService.RetrieveUserByIdAsync(id);

        if (currentUser.Role == "Admin")
        {
            if (existing.TenantId != currentUser.TenantId)
                return Forbid();

            if (userDto.Role == Role.SuperAdmin)
                return Forbid("Siz SuperAdmin rolini bera olmaysiz");

            var allowedRoles = new[] { Role.Teacher, Role.Secretary, Role.Student };
            if (!allowedRoles.Contains(userDto.Role))
                return Forbid("Bu rolga o‘zgartirishga ruxsat yo‘q");
        }

        if (currentUser.Role == "SuperAdmin")
        {
            if (existing.Role != Role.Admin)
                return Forbid("SuperAdmin faqat Admin foydalanuvchisini yangilashi mumkin");

            if (userDto.Role != Role.Admin)
                return Forbid("SuperAdmin roli Admin bo‘lishi kerak");
        }

        userDto.TenantId = existing.TenantId;
        var updatedDto = await userService.ModifyUserAsync(userDto);
        return Ok(updatedDto);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var existing = await userService.RetrieveUserByIdAsync(id);

        if (existing.Role == Role.SuperAdmin)
            return Forbid("SuperAdmin foydalanuvchisi o‘chirilmaydi");

        if (currentUser.Role == "Admin" && existing.TenantId != currentUser.TenantId)
            return Forbid("Bu foydalanuvchiga ruxsatingiz yo‘q");

        await userService.RemoveUserAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/toggle-Status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        return Ok(await userService.ToggleUserStatusAsync(id));
    }

}
