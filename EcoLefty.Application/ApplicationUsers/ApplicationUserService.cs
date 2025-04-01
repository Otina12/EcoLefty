using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Common.Images;
using EcoLefty.Domain.Common;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.ApplicationUsers;

public class ApplicationUserService : IApplicationUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public ApplicationUserService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task<IEnumerable<ApplicationUserResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(trackChanges: false, token: token, ApplicationUserIncludes.Account);
        return _mapper.Map<IEnumerable<ApplicationUserResponseDto>>(users);
    }

    public async Task<ApplicationUserDetailsResponseDto> GetByIdAsync(string id, CancellationToken token)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: false, token: token,
            ApplicationUserIncludes.Account,
            ApplicationUserIncludes.Purchases,
            ApplicationUserIncludes.Categories);

        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }

        return _mapper.Map<ApplicationUserDetailsResponseDto>(user);
    }

    public async Task<TokenResponseDto> CreateAsync(CreateApplicationUserRequestDto createUserDto, CancellationToken token = default)
    {
        // We need a transaction to ensure that identity user is not added without application user and vice versa
        using var transaction = await _unitOfWork.BeginTransactionAsync(token);

        try
        {
            // First, we need to register identity user
            var accountDto = _mapper.Map<RegisterAccountRequestDto>(createUserDto);
            TokenResponseDto tokenPair = await _authenticationService.RegisterAccountAsync(accountDto, AccountRole.User);
            string accountId = await _authenticationService.GetAccountIdFromJwtTokenAsync(tokenPair.AccessToken);

            // Then we can create the application user
            var applicationUser = _mapper.Map<ApplicationUser>(createUserDto);
            applicationUser.Id = accountId;

            var imageUrl = createUserDto.ProfilePictureFile is null ?
               Constants.DEFAULT_PROFILE_PICTURE_PATH : await _imageService.UploadImageAsync(createUserDto.ProfilePictureFile, token);

            applicationUser.ProfilePictureUrl = imageUrl;

            await _unitOfWork.Users.CreateAsync(applicationUser, token);
            await _unitOfWork.SaveChangesAsync(token);

            await transaction.CommitAsync(token);

            return tokenPair;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(); // without token
            throw;
        }
    }

    public async Task<ApplicationUserResponseDto> UpdateAsync(string id, UpdateApplicationUserRequestDto updateUserDto, CancellationToken token = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: true, token: token);
        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }
        _mapper.Map(updateUserDto, user);

        if (updateUserDto.ProfilePictureFile is not null)
        {
            var imageUrl = await _imageService.UploadImageAsync(updateUserDto.ProfilePictureFile, token);
            user.ProfilePictureUrl = imageUrl;
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<ApplicationUserResponseDto>(user);
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ApplicationUserNotFoundException"></exception>
    public async Task<bool> DeleteAsync(string id, CancellationToken token = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, trackChanges: false, token: token);
        if (user is null)
        {
            throw new ApplicationUserNotFoundException(id);
        }

        _unitOfWork.Users.Delete(user);

        // Because of soft delete, we need deactivate related account manually.
        // Other entities will be soft deleted using custom cascading soft delete when saving changes. Account entity is not soft-deletable (by choice).
        await _unitOfWork.Accounts.DeactivateAsync(user.Id, token);

        var result = await _unitOfWork.SaveChangesAsync(token);
        return result > 0;
    }
}
