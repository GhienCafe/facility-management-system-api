using AppCore.Extensions;
using MainData;
using MainData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_FFMS.Controllers;

public class InitController : BaseController
{
    private readonly MainUnitOfWork _unitOfWork;

    public InitController(MainUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var salt = SecurityExtension.GenerateSalt();
        var user = new User
        {
            Id = Guid.NewGuid(),
            DepartmentId = Guid.Parse("e17aea39-6fd3-48d6-9d52-0b879132c788"),
            UserCode = "SE151129", // Gán giá trị UserCode ở đây
            Email = "trieudhse151129@fpt.edu.vn",
            Address = "Thu Duc, TP Ho Chi Minh",
            Fullname = "Do Hai Trieu",
            Gender = true,
            Password = SecurityExtension.HashPassword<User>("123456", salt),
            Role = UserRole.GlobalManager,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0914567635",
        };

        await _unitOfWork.UserRepository.InsertAsync(user, Guid.Empty, DateTime.UtcNow);
        return Ok();
    }
}