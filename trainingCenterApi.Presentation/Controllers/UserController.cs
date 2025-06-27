using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            this.userService = userService ?? throw new NullArgumentException(nameof(userService));
            this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            try
            {
                var user = mapper.Map<User>(userDto);
                var createdUser = await userService.RegisterUserAsync(user);
                var resultDto = mapper.Map<UserDto>(createdUser);
                return CreatedAtAction(nameof(GetUserById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var users = await userService.RetrieveAllUsersAsync();
            var totalCount = users.Count;
            var pagedUsers = users.Skip((page - 1) * size).Take(size).ToList();
            var resultDtos = mapper.Map<List<UserDto>>(pagedUsers);

            var result = new PagedResult<UserDto>
            {
                Items = resultDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = size
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await userService.RetrieveUserByIdAsync(id);
            var resultDto = mapper.Map<UserDto>(user);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userDto)
        {
            if (id != userDto.Id)
                throw new ArgumentException("ID mismatch.");

            var user = mapper.Map<User>(userDto);
            var updatedUser = await userService.ModifyUserAsync(user);
            var resultDto = mapper.Map<UserDto>(updatedUser);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userService.RemoveUserAsync(id);
            return NoContent();
        }
    }
}