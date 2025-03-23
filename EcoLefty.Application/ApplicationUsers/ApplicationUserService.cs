using AutoMapper;
using EcoLefty.Application.Accounts;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.ApplicationUsers;

public class ApplicationUserService : IApplicationUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _authService;
    private readonly IMapper _mapper;

    public ApplicationUserService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService authService)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ApplicationUserResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(trackChanges: false, token: token, ApplicationUserIncludeExpressions.Account);
        return _mapper.Map<IEnumerable<ApplicationUserResponseDto>>(users);
    }

    public async Task<ApplicationUserDetailsResponseDto> GetByIdAsync(int id, CancellationToken token)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: false, token: token,
            ApplicationUserIncludeExpressions.Account,
            ApplicationUserIncludeExpressions.Categories);

        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }

        return _mapper.Map<ApplicationUserDetailsResponseDto>(user);
    }

    public async Task<ApplicationUserResponseDto> CreateAsync(CreateApplicationUserRequestDto createUserDto, CancellationToken token = default)
    {
        // We need a transaction to ensure that identity user is not added without application user and vice versa
        using var transaction = await _unitOfWork.BeginTransactionAsync(token);

        try
        {
            // First, we need to register identity user
            var accountDto = _mapper.Map<RegisterAccountRequestDto>(createUserDto);
            string accountId = await _authService.RegisterAccountAsync(accountDto);

            // Then we can create the application user
            var applicationUser = _mapper.Map<ApplicationUser>(createUserDto);
            applicationUser.AccountId = accountId;

            await _unitOfWork.Users.CreateAsync(applicationUser);
            await _unitOfWork.SaveChangesAsync(token);

            await transaction.CommitAsync(token);

            return _mapper.Map<ApplicationUserResponseDto>(applicationUser);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(); // without token
            throw;
        }
    }

    public async Task<ApplicationUserResponseDto> UpdateAsync(int id, UpdateApplicationUserRequestDto updateUserDto, CancellationToken token = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: true, token: token);
        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }

        _mapper.Map(updateUserDto, user);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<ApplicationUserResponseDto>(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: false, token: token);
        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }

        _unitOfWork.Users.Delete(user);

        // Because of soft delete, we need delete/soft delete related entities
        await _unitOfWork.Accounts.DeactivateAsync(user.AccountId, token);

        var result = await _unitOfWork.SaveChangesAsync(token);
        return result > 0;
    }
}
