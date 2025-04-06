using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Offers;
using EcoLefty.Domain.Common;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Common.IncludeExpressions;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Entities;

namespace EcoLefty.Application.Companies;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionWrapper _transactionWrapper;
    private readonly IOfferService _offerService;
    private readonly IImageService _imageService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public CompanyService(
        IUnitOfWork unitOfWork,
        ITransactionWrapper transactionWrapper,
        IOfferService offerService,
        IImageService imageService,
        IMapper mapper,
        IAuthenticationService authenticationService)
    {
        _unitOfWork = unitOfWork;
        _transactionWrapper = transactionWrapper;
        _offerService = offerService;
        _imageService = imageService;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetAllAsync(CancellationToken token = default)
    {
        var companies = await _unitOfWork.Companies.GetAllAsync(trackChanges: false, token: token, CompanyIncludes.Account);
        return _mapper.Map<IEnumerable<CompanyResponseDto>>(companies);
    }

    public async Task<CompanyDetailsResponseDto> GetByIdAsync(string id, CancellationToken token = default, bool includeArchivedOffers = false)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, trackChanges: false, token: token,
            CompanyIncludes.Account,
            CompanyIncludes.Products
            );

        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }

        var companyDetailsDto = _mapper.Map<CompanyDetailsResponseDto>(company);

        var companyOffers = includeArchivedOffers ?
            await _offerService.GetAllOffersOfCompanyAsync(id, token)
            : await _offerService.GetActiveOffersOfCompanyAsync(id, token);

        companyDetailsDto.Offers = companyOffers;
        return companyDetailsDto;
    }

    public async Task<TokenResponseDto> CreateAsync(CreateCompanyRequestDto createCompanyDto, CancellationToken token = default)
    {
        // We need a transaction to ensure that identity user is not added without company
        await _transactionWrapper.BeginTransactionAsync(token);

        try
        {
            // First, we need to register identity user
            var accountDto = _mapper.Map<RegisterAccountRequestDto>(createCompanyDto);
            TokenResponseDto tokenPair = await _authenticationService.RegisterAccountAsync(accountDto, AccountRole.Company);
            var accountId = await _authenticationService.GetAccountIdFromJwtTokenAsync(tokenPair.AccessToken);

            // Then we can create the company
            var company = _mapper.Map<Company>(createCompanyDto);
            company.Id = accountId;

            var imageUrl = createCompanyDto.LogoFile is null ?
                Constants.DEFAULT_COMPANY_IMAGE_PATH : await _imageService.UploadImageAsync(createCompanyDto.LogoFile, token);

            company.LogoUrl = imageUrl;

            await _unitOfWork.Companies.CreateAsync(company);
            await _unitOfWork.SaveChangesAsync(token);

            await _transactionWrapper.CommitTransactionAsync(); // without token

            return tokenPair;
        }
        catch (Exception)
        {
            await _transactionWrapper.RollbackTransactionAsync(); // without token
            throw;
        }
    }

    public async Task<CompanyResponseDto> UpdateAsync(string companyId, UpdateCompanyRequestDto updateCompanyDto, CancellationToken token = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId, trackChanges: true, token: token);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        if (companyId != _unitOfWork.CurrentUserContext.UserId)
        {
            throw new ForbiddenException();
        }

        _mapper.Map(updateCompanyDto, company);

        if (updateCompanyDto.LogoFile is not null)
        {
            var imageUrl = await _imageService.UploadImageAsync(updateCompanyDto.LogoFile, token);
            company.LogoUrl = imageUrl;
        }

        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync(token);

        return _mapper.Map<CompanyResponseDto>(company);
    }

    public async Task<bool> ApproveCompanyAsync(string id, CancellationToken token = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, trackChanges: true, token: token);
        if (company is null)
        {
            throw new CompanyNotFoundException(id);
        }

        company.IsApproved = true;

        var account = await _unitOfWork.Accounts.GetByIdAsync(id, false, token);
        if (account is null)
        {
            throw new CompanyNotFoundException(id);
        }

        await _authenticationService.AddClaimAsync(account, "IsApproved", "True");

        return await _unitOfWork.SaveChangesAsync(token) > 0;
    }

    /// <summary>
    /// Soft deletes an entity and all related entities.
    /// </summary>
    /// <param name="companyId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="CompanyNotFoundException"></exception>
    public async Task<bool> DeleteAsync(string companyId, CancellationToken token = default)
    {
        bool isAdmin = _unitOfWork.CurrentUserContext.IsInRole("Admin");
        bool isOwner = companyId == _unitOfWork.CurrentUserContext.UserId;

        if (!isAdmin && !isOwner)
        {
            throw new UnauthorizedException();
        }

        var company = await _unitOfWork.Companies.GetByIdAsync(companyId, trackChanges: false, token: token);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        _unitOfWork.Companies.Delete(company);

        // Because of soft delete, we need deactivate related account manually. Account entity is not soft-deletable (by choice).
        // Other entities will be soft deleted using custom cascading soft delete when saving changes.
        await _unitOfWork.Accounts.DeactivateAsync(company.Id, token);

        var deleted = await _unitOfWork.SaveChangesAsync(token);
        return deleted > 0;
    }
}
