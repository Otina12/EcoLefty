using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities;
using EcoLefty.Domain.Entities.Auth;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Tests.Application.TestHelpers;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Linq.Expressions;

namespace EcoLefty.Tests.Mocks;

public class UnitOfWorkMock
{
    private IAccountRepository? _accountRepository;
    private IApplicationUserRepository? _userRepository;
    private ICategoryRepository? _categoryRepository;
    private ICompanyRepository? _companyRepository;
    private IOfferRepository? _offerRepository;
    private IProductRepository? _productRepository;
    private IPurchaseRepository? _purchaseRepository;
    private IAuditLogRepository? _auditLogRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private ICurrentUserContext? _currentUserContext;

    public IUnitOfWork UnitOfWorkInstance { get; }

    // Store generated test data
    public List<Account> GeneratedAccounts { get; private set; } = new List<Account>();
    public List<ApplicationUser> GeneratedUsers { get; private set; } = new List<ApplicationUser>();
    public List<Company> GeneratedCompanies { get; private set; } = new List<Company>();
    public List<Category> GeneratedCategories { get; private set; } = new List<Category>();
    public List<Product> GeneratedProducts { get; private set; } = new List<Product>();
    public List<Offer> GeneratedOffers { get; private set; } = new List<Offer>();
    public List<Purchase> GeneratedPurchases { get; private set; } = new List<Purchase>();
    public List<RefreshToken> GeneratedRefreshTokens { get; private set; } = new List<RefreshToken>();
    public List<AuditLog> GeneratedAuditLogs { get; private set; } = new List<AuditLog>();

    public UnitOfWorkMock()
    {
        var scenario = TestDataGenerator.GenerateCompleteScenario(5, 3, 2, 2, 3);
        GeneratedUsers = scenario.Customers;
        GeneratedCompanies = scenario.Companies;
        GeneratedProducts = scenario.Products;
        GeneratedOffers = scenario.Offers;
        GeneratedPurchases = scenario.Purchases;

        GeneratedAccounts = GeneratedUsers.Select(u => u.Account).Concat(
            GeneratedCompanies.Select(c => c.Account)).ToList();

        GeneratedCategories = GeneratedProducts
            .SelectMany(p => p.Categories)
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .ToList();

        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(u => u.Accounts).Returns(GetAccountRepository());
        mockUnitOfWork.Setup(u => u.Users).Returns(GetUserRepository());
        mockUnitOfWork.Setup(u => u.Categories).Returns(GetCategoryRepository());
        mockUnitOfWork.Setup(u => u.Companies).Returns(GetCompanyRepository());
        mockUnitOfWork.Setup(u => u.Offers).Returns(GetOfferRepository());
        mockUnitOfWork.Setup(u => u.Products).Returns(GetProductRepository());
        mockUnitOfWork.Setup(u => u.Purchases).Returns(GetPurchaseRepository());
        mockUnitOfWork.Setup(u => u.CurrentUserContext).Returns(GetCurrentUserContext());

        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Returns(Task.FromResult(1));

        // Setup BeginTransactionAsync
        mockUnitOfWork.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Mock.Of<IDbContextTransaction>()));

        UnitOfWorkInstance = mockUnitOfWork.Object;
    }

    #region Repository Mocks
    public IAccountRepository GetAccountRepository()
    {
        if (_accountRepository != null)
            return _accountRepository;

        var mockRepository = new Mock<IAccountRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Account, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Account, object>>[] includes) =>
                Task.FromResult(GeneratedAccounts.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Account, object>>[]>()))
            .Returns((Expression<Func<Account, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Account, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var accounts = GeneratedAccounts.Where(predicate);
                return Task.FromResult(accounts.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Account, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Account, object>>[] includes) =>
                GeneratedAccounts.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Account, object>>[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, Expression<Func<Account, object>>[] includes) =>
            {
                var account = GeneratedAccounts.FirstOrDefault(a => a.Id == id);
                return Task.FromResult(account);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var account = GeneratedAccounts.FirstOrDefault(a => a.Id == id);
                return Task.FromResult(account);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Account, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Account, object>>[]>()))
            .Returns((Expression<Func<Account, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Account, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var account = GeneratedAccounts.FirstOrDefault(predicate);
                return Task.FromResult(account);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string id, CancellationToken token) =>
            {
                var exists = GeneratedAccounts.Any(a => a.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .Callback((Account account, CancellationToken token) =>
            {
                GeneratedAccounts.Add(account);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Account>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Account> accounts, CancellationToken token) =>
            {
                GeneratedAccounts.AddRange(accounts);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Account>()))
            .Callback((Account account) =>
            {
                var existingAccount = GeneratedAccounts.FirstOrDefault(a => a.Id == account.Id);
                if (existingAccount != null)
                {
                    var index = GeneratedAccounts.IndexOf(existingAccount);
                    GeneratedAccounts[index] = account;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Account>()))
            .Callback((Account account) =>
            {
                var existingAccount = GeneratedAccounts.FirstOrDefault(a => a.Id == account.Id);
                if (existingAccount != null)
                {
                    GeneratedAccounts.Remove(existingAccount);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Account>>()))
            .Callback((IEnumerable<Account> accounts) =>
            {
                foreach (var account in accounts)
                {
                    var existingAccount = GeneratedAccounts.FirstOrDefault(a => a.Id == account.Id);
                    if (existingAccount != null)
                    {
                        GeneratedAccounts.Remove(existingAccount);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Account, bool>>>()))
            .Callback((Expression<Func<Account, bool>> where) =>
            {
                var predicate = where.Compile();
                var accountsToDelete = GeneratedAccounts.Where(predicate).ToList();
                foreach (var account in accountsToDelete)
                {
                    GeneratedAccounts.Remove(account);
                }
            });

        _accountRepository = mockRepository.Object;
        return _accountRepository;
    }

    public IApplicationUserRepository GetUserRepository()
    {
        if (_userRepository != null)
            return _userRepository;

        var mockRepository = new Mock<IApplicationUserRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<ApplicationUser, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<ApplicationUser, object>>[] includes) =>
                Task.FromResult(GeneratedUsers.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<ApplicationUser, object>>[]>()))
            .Returns((Expression<Func<ApplicationUser, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<ApplicationUser, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var users = GeneratedUsers.Where(predicate);
                return Task.FromResult(users.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<ApplicationUser, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<ApplicationUser, object>>[] includes) =>
                GeneratedUsers.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<ApplicationUser, object>>[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, Expression<Func<ApplicationUser, object>>[] includes) =>
            {
                var user = GeneratedUsers.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var user = GeneratedUsers.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<ApplicationUser, object>>[]>()))
            .Returns((Expression<Func<ApplicationUser, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<ApplicationUser, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var user = GeneratedUsers.FirstOrDefault(predicate);
                return Task.FromResult(user);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string id, CancellationToken token) =>
            {
                var exists = GeneratedUsers.Any(u => u.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
            .Callback((ApplicationUser user, CancellationToken token) =>
            {
                GeneratedUsers.Add(user);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ApplicationUser>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<ApplicationUser> users, CancellationToken token) =>
            {
                GeneratedUsers.AddRange(users);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<ApplicationUser>()))
            .Callback((ApplicationUser user) =>
            {
                var existingUser = GeneratedUsers.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    var index = GeneratedUsers.IndexOf(existingUser);
                    GeneratedUsers[index] = user;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<ApplicationUser>()))
            .Callback((ApplicationUser user) =>
            {
                var existingUser = GeneratedUsers.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    GeneratedUsers.Remove(existingUser);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<ApplicationUser>>()))
            .Callback((IEnumerable<ApplicationUser> users) =>
            {
                foreach (var user in users)
                {
                    var existingUser = GeneratedUsers.FirstOrDefault(u => u.Id == user.Id);
                    if (existingUser != null)
                    {
                        GeneratedUsers.Remove(existingUser);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
            .Callback((Expression<Func<ApplicationUser, bool>> where) =>
            {
                var predicate = where.Compile();
                var usersToDelete = GeneratedUsers.Where(predicate).ToList();
                foreach (var user in usersToDelete)
                {
                    GeneratedUsers.Remove(user);
                }
            });

        _userRepository = mockRepository.Object;
        return _userRepository;
    }

    public ICategoryRepository GetCategoryRepository()
    {
        if (_categoryRepository != null)
            return _categoryRepository;

        var mockRepository = new Mock<ICategoryRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Category, object>>[] includes) =>
                Task.FromResult(GeneratedCategories.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .Returns((Expression<Func<Category, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Category, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var categories = GeneratedCategories.Where(predicate);
                return Task.FromResult(categories.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Category, object>>[] includes) =>
                GeneratedCategories.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, Expression<Func<Category, object>>[] includes) =>
            {
                var category = GeneratedCategories.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(category);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var category = GeneratedCategories.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(category);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .Returns((Expression<Func<Category, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Category, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var category = GeneratedCategories.FirstOrDefault(predicate);
                return Task.FromResult(category);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((int id, CancellationToken token) =>
            {
                var exists = GeneratedCategories.Any(c => c.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Callback((Category category, CancellationToken token) =>
            {
                GeneratedCategories.Add(category);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Category>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Category> categories, CancellationToken token) =>
            {
                GeneratedCategories.AddRange(categories);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Category>()))
            .Callback((Category category) =>
            {
                var existingCategory = GeneratedCategories.FirstOrDefault(c => c.Id == category.Id);
                if (existingCategory != null)
                {
                    var index = GeneratedCategories.IndexOf(existingCategory);
                    GeneratedCategories[index] = category;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Category>()))
            .Callback((Category category) =>
            {
                var existingCategory = GeneratedCategories.FirstOrDefault(c => c.Id == category.Id);
                if (existingCategory != null)
                {
                    GeneratedCategories.Remove(existingCategory);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Category>>()))
            .Callback((IEnumerable<Category> categories) =>
            {
                foreach (var category in categories)
                {
                    var existingCategory = GeneratedCategories.FirstOrDefault(c => c.Id == category.Id);
                    if (existingCategory != null)
                    {
                        GeneratedCategories.Remove(existingCategory);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Category, bool>>>()))
            .Callback((Expression<Func<Category, bool>> where) =>
            {
                var predicate = where.Compile();
                var categoriesToDelete = GeneratedCategories.Where(predicate).ToList();
                foreach (var category in categoriesToDelete)
                {
                    GeneratedCategories.Remove(category);
                }
            });

        _categoryRepository = mockRepository.Object;
        return _categoryRepository;
    }

    public ICompanyRepository GetCompanyRepository()
    {
        if (_companyRepository != null)
            return _companyRepository;

        var mockRepository = new Mock<ICompanyRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Company, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Company, object>>[] includes) =>
                Task.FromResult(GeneratedCompanies.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Company, object>>[]>()))
            .Returns((Expression<Func<Company, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Company, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var companies = GeneratedCompanies.Where(predicate);
                return Task.FromResult(companies.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Company, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Company, object>>[] includes) =>
                GeneratedCompanies.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Company, object>>[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, Expression<Func<Company, object>>[] includes) =>
            {
                var company = GeneratedCompanies.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(company);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((string id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var company = GeneratedCompanies.FirstOrDefault(c => c.Id == id);
                return Task.FromResult(company);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Company, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Company, object>>[]>()))
            .Returns((Expression<Func<Company, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Company, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var company = GeneratedCompanies.FirstOrDefault(predicate);
                return Task.FromResult(company);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string id, CancellationToken token) =>
            {
                var exists = GeneratedCompanies.Any(c => c.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .Callback((Company company, CancellationToken token) =>
            {
                GeneratedCompanies.Add(company);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Company>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Company> companies, CancellationToken token) =>
            {
                GeneratedCompanies.AddRange(companies);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Company>()))
            .Callback((Company company) =>
            {
                var existingCompany = GeneratedCompanies.FirstOrDefault(c => c.Id == company.Id);
                if (existingCompany != null)
                {
                    var index = GeneratedCompanies.IndexOf(existingCompany);
                    GeneratedCompanies[index] = company;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Company>()))
            .Callback((Company company) =>
            {
                var existingCompany = GeneratedCompanies.FirstOrDefault(c => c.Id == company.Id);
                if (existingCompany != null)
                {
                    GeneratedCompanies.Remove(existingCompany);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Company>>()))
            .Callback((IEnumerable<Company> companies) =>
            {
                foreach (var company in companies)
                {
                    var existingCompany = GeneratedCompanies.FirstOrDefault(c => c.Id == company.Id);
                    if (existingCompany != null)
                    {
                        GeneratedCompanies.Remove(existingCompany);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Company, bool>>>()))
            .Callback((Expression<Func<Company, bool>> where) =>
            {
                var predicate = where.Compile();
                var companiesToDelete = GeneratedCompanies.Where(predicate).ToList();
                foreach (var company in companiesToDelete)
                {
                    GeneratedCompanies.Remove(company);
                }
            });

        _companyRepository = mockRepository.Object;
        return _companyRepository;
    }

    public IOfferRepository GetOfferRepository()
    {
        if (_offerRepository != null)
            return _offerRepository;

        var mockRepository = new Mock<IOfferRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Offer, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Offer, object>>[] includes) =>
                Task.FromResult(GeneratedOffers.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Offer, object>>[]>()))
            .Returns((Expression<Func<Offer, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Offer, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var offers = GeneratedOffers.Where(predicate);
                return Task.FromResult(offers.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Offer, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Offer, object>>[] includes) =>
                GeneratedOffers.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Offer, object>>[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, Expression<Func<Offer, object>>[] includes) =>
            {
                var offer = GeneratedOffers.FirstOrDefault(o => o.Id == id);
                return Task.FromResult(offer);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var offer = GeneratedOffers.FirstOrDefault(o => o.Id == id);
                return Task.FromResult(offer);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Offer, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Offer, object>>[]>()))
            .Returns((Expression<Func<Offer, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Offer, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var offer = GeneratedOffers.FirstOrDefault(predicate);
                return Task.FromResult(offer);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((int id, CancellationToken token) =>
            {
                var exists = GeneratedOffers.Any(o => o.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .Callback((Offer offer, CancellationToken token) =>
            {
                GeneratedOffers.Add(offer);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Offer>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Offer> offers, CancellationToken token) =>
            {
                GeneratedOffers.AddRange(offers);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Offer>()))
            .Callback((Offer offer) =>
            {
                var existingOffer = GeneratedOffers.FirstOrDefault(o => o.Id == offer.Id);
                if (existingOffer != null)
                {
                    var index = GeneratedOffers.IndexOf(existingOffer);
                    GeneratedOffers[index] = offer;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Offer>()))
            .Callback((Offer offer) =>
            {
                var existingOffer = GeneratedOffers.FirstOrDefault(o => o.Id == offer.Id);
                if (existingOffer != null)
                {
                    GeneratedOffers.Remove(existingOffer);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Offer>>()))
            .Callback((IEnumerable<Offer> offers) =>
            {
                foreach (var offer in offers)
                {
                    var existingOffer = GeneratedOffers.FirstOrDefault(o => o.Id == offer.Id);
                    if (existingOffer != null)
                    {
                        GeneratedOffers.Remove(existingOffer);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Offer, bool>>>()))
            .Callback((Expression<Func<Offer, bool>> where) =>
            {
                var predicate = where.Compile();
                var offersToDelete = GeneratedOffers.Where(predicate).ToList();
                foreach (var offer in offersToDelete)
                {
                    GeneratedOffers.Remove(offer);
                }
            });

        _offerRepository = mockRepository.Object;
        return _offerRepository;
    }

    public IProductRepository GetProductRepository()
    {
        if (_productRepository != null)
            return _productRepository;

        var mockRepository = new Mock<IProductRepository>();

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Product, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Product, object>>[] includes) =>
                Task.FromResult(GeneratedProducts.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Product, object>>[]>()))
            .Returns((Expression<Func<Product, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Product, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var products = GeneratedProducts.Where(predicate);
                return Task.FromResult(products.AsEnumerable());
            });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Product, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Product, object>>[] includes) =>
                GeneratedProducts.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Product, object>>[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, Expression<Func<Product, object>>[] includes) =>
            {
                var product = GeneratedProducts.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var product = GeneratedProducts.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(product);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Product, object>>[]>()))
            .Returns((Expression<Func<Product, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Product, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var product = GeneratedProducts.FirstOrDefault(predicate);
                return Task.FromResult(product);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((int id, CancellationToken token) =>
            {
                var exists = GeneratedProducts.Any(p => p.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback((Product product, CancellationToken token) =>
            {
                GeneratedProducts.Add(product);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Product> products, CancellationToken token) =>
            {
                GeneratedProducts.AddRange(products);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Product>()))
            .Callback((Product product) =>
            {
                var existingProduct = GeneratedProducts.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    var index = GeneratedProducts.IndexOf(existingProduct);
                    GeneratedProducts[index] = product;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Product>()))
            .Callback((Product product) =>
            {
                var existingProduct = GeneratedProducts.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    GeneratedProducts.Remove(existingProduct);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Product>>()))
            .Callback((IEnumerable<Product> products) =>
            {
                foreach (var product in products)
                {
                    var existingProduct = GeneratedProducts.FirstOrDefault(p => p.Id == product.Id);
                    if (existingProduct != null)
                    {
                        GeneratedProducts.Remove(existingProduct);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Product, bool>>>()))
            .Callback((Expression<Func<Product, bool>> where) =>
            {
                var predicate = where.Compile();
                var productsToDelete = GeneratedProducts.Where(predicate).ToList();
                foreach (var product in productsToDelete)
                {
                    GeneratedProducts.Remove(product);
                }
            });

        _productRepository = mockRepository.Object;
        return _productRepository;
    }

    public IPurchaseRepository GetPurchaseRepository()
    {
        if (_purchaseRepository != null)
            return _purchaseRepository;

        var mockRepository = new Mock<IPurchaseRepository>();

        mockRepository.Setup(r => r.CancelAllPurchasesByOfferAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((int offerId, CancellationToken token) =>
            {
                var purchases = GeneratedPurchases.Where(p => p.OfferId == offerId);
                foreach (var purchase in purchases)
                {
                    purchase.PurchaseStatus = PurchaseStatus.Cancelled;
                }

                return Task.CompletedTask;
            });

        // GetAllAsync
        mockRepository.Setup(r => r.GetAllAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Purchase, object>>[]>()))
            .Returns((bool trackChanges, CancellationToken token, Expression<Func<Purchase, object>>[] includes) =>
                Task.FromResult(GeneratedPurchases.AsEnumerable()));

        // GetAllWhereAsync
        mockRepository.Setup(r => r.GetAllWhereAsync(
            It.IsAny<Expression<Func<Purchase, bool>>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Purchase, object>>[]>()))
        .Returns((Expression<Func<Purchase, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Purchase, object>>[] includes) =>
        {
            var predicate = where.Compile();
            var filtered = GeneratedPurchases.Where(predicate).ToList();
            return Task.FromResult<IEnumerable<Purchase>>(filtered);
        });

        // GetAllAsQueryable
        mockRepository.Setup(r => r.GetAllAsQueryable(It.IsAny<bool>(), It.IsAny<Expression<Func<Purchase, object>>[]>()))
            .Returns((bool trackChanges, Expression<Func<Purchase, object>>[] includes) =>
                GeneratedPurchases.AsQueryable());

        // GetByIdAsync
        mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Purchase, object>>[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, Expression<Func<Purchase, object>>[] includes) =>
            {
                var purchase = GeneratedPurchases.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(purchase);
            });

        // GetByIdIncludeStringsAsync
        mockRepository.Setup(r => r.GetByIdIncludeStringsAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<string[]>()))
            .Returns((int id, bool trackChanges, CancellationToken token, string[] includes) =>
            {
                var purchase = GeneratedPurchases.FirstOrDefault(p => p.Id == id);
                return Task.FromResult(purchase);
            });

        // GetOneWhereAsync
        mockRepository.Setup(r => r.GetOneWhereAsync(It.IsAny<Expression<Func<Purchase, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Purchase, object>>[]>()))
            .Returns((Expression<Func<Purchase, bool>> where, bool trackChanges, CancellationToken token, Expression<Func<Purchase, object>>[] includes) =>
            {
                var predicate = where.Compile();
                var purchase = GeneratedPurchases.FirstOrDefault(predicate);
                return Task.FromResult(purchase);
            });

        // ExistsAsync
        mockRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((int id, CancellationToken token) =>
            {
                var exists = GeneratedPurchases.Any(p => p.Id == id);
                return Task.FromResult(exists);
            });

        // CreateAsync
        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Purchase>(), It.IsAny<CancellationToken>()))
            .Callback((Purchase purchase, CancellationToken token) =>
            {
                GeneratedPurchases.Add(purchase);
            })
            .Returns(Task.CompletedTask);

        // CreateRangeAsync
        mockRepository.Setup(r => r.CreateRangeAsync(It.IsAny<IEnumerable<Purchase>>(), It.IsAny<CancellationToken>()))
            .Callback((IEnumerable<Purchase> purchases, CancellationToken token) =>
            {
                GeneratedPurchases.AddRange(purchases);
            })
            .Returns(Task.CompletedTask);

        // Update
        mockRepository.Setup(r => r.Update(It.IsAny<Purchase>()))
            .Callback((Purchase purchase) =>
            {
                var existingPurchase = GeneratedPurchases.FirstOrDefault(p => p.Id == purchase.Id);
                if (existingPurchase != null)
                {
                    var index = GeneratedPurchases.IndexOf(existingPurchase);
                    GeneratedPurchases[index] = purchase;
                }
            });

        // Delete
        mockRepository.Setup(r => r.Delete(It.IsAny<Purchase>()))
            .Callback((Purchase purchase) =>
            {
                var existingPurchase = GeneratedPurchases.FirstOrDefault(p => p.Id == purchase.Id);
                if (existingPurchase != null)
                {
                    GeneratedPurchases.Remove(existingPurchase);
                }
            });

        // DeleteRange
        mockRepository.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<Purchase>>()))
            .Callback((IEnumerable<Purchase> purchases) =>
            {
                foreach (var purchase in purchases)
                {
                    var existingPurchase = GeneratedPurchases.FirstOrDefault(p => p.Id == purchase.Id);
                    if (existingPurchase != null)
                    {
                        GeneratedPurchases.Remove(existingPurchase);
                    }
                }
            });

        // DeleteAllWhere
        mockRepository.Setup(r => r.DeleteAllWhere(It.IsAny<Expression<Func<Purchase, bool>>>()))
            .Callback((Expression<Func<Purchase, bool>> where) =>
            {
                var predicate = where.Compile();
                var purchasesToDelete = GeneratedPurchases.Where(predicate).ToList();
                foreach (var purchase in purchasesToDelete)
                {
                    GeneratedPurchases.Remove(purchase);
                }
            });

        _purchaseRepository = mockRepository.Object;
        return _purchaseRepository;
    }

    public ICurrentUserContext GetCurrentUserContext()
    {
        if (_currentUserContext != null)
            return _currentUserContext;

        var mockCurrentUserContext = new Mock<ICurrentUserContext>();

        mockCurrentUserContext.Setup(x => x.UserId).Returns("test-user-id");
        mockCurrentUserContext.Setup(x => x.UserRole).Returns("Admin");

        mockCurrentUserContext.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns((string roleName) => roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase));

        _currentUserContext = mockCurrentUserContext.Object;
        return _currentUserContext;
    }


    #endregion

}