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
        // Create campus
        var campus = new Campus
        {
            Id = Guid.NewGuid(),
            CampusName = "Hồ Chí Minh Campus",
            Address = "Tp Hồ Chí Minh",
            Description = "Không cần mô tả",
            Telephone = "0977627412"
        };

        await _unitOfWork.CampusRepository.InsertAsync(campus, Guid.Empty, DateTime.UtcNow);

        var department = new Department
        {
            Id = Guid.NewGuid(),
            CampusId = campus.Id,
            DepartmentCode = "DE001",
            DepartmentName = "Ban quản lý"
        };

        await _unitOfWork.DepartmentRepository.InsertAsync(department, Guid.Empty, DateTime.UtcNow);
        
        var salt = SecurityExtension.GenerateSalt();
        var user = new User
        {
            Id = Guid.NewGuid(),
            DepartmentId = department.Id,
            UserCode = "SE150747", // Gán giá trị UserCode ở đây
            Email = "giangntse150747@fpt.edu.vn",
            Address = "Thu Duc, TP Ho Chi Minh",
            Fullname = "Nguyen Truong Giang",
            Gender = true,
            Password = SecurityExtension.HashPassword<User>("Giang123@", salt),
            Role = UserRole.Administrator,
            Salt = salt,
            Status = UserStatus.Active,
            PhoneNumber = "0977264752",
        };

        await _unitOfWork.UserRepository.InsertAsync(user, Guid.Empty, DateTime.UtcNow);
        return Ok();
    }
}