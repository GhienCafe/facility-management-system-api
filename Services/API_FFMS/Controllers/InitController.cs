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
            Email = "giangntse150747@fpt.edu.vn",
            Address = "Thu Duc, TP Ho Chi Minh",
            Fullname = "Nguyen Truong Giang",
            Password = SecurityExtension.HashPassword<User>("Giang123@", salt),
            Role = UserRole.Manager,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264752",
        };

        await _unitOfWork.UserRepository.InsertAsync(user, Guid.Empty, DateTime.UtcNow);
        return Ok();
    }
}