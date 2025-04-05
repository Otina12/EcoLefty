using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Infrastructure.Repositories;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace EcoLefty.Infrastructure;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly EcoLeftyDbContext _context;

    private readonly Lazy<IAccountRepository> _accountRepository;
    private readonly Lazy<IApplicationUserRepository> _userRepository;
    private readonly Lazy<ICategoryRepository> _categoryRepository;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly Lazy<IOfferRepository> _offerRepository;
    private readonly Lazy<IProductRepository> _productRepository;
    private readonly Lazy<IPurchaseRepository> _purchaseRepository;
    private readonly Lazy<IAuditLogRepository> _auditLogRepository;
    private readonly Lazy<IRefreshTokenRepository> _refreshTokenRepository;

    private readonly ICurrentUserContext _currentUserContext;

    public UnitOfWork(EcoLeftyDbContext ecoLeftyContext, ICurrentUserContext userContext)
    {
        _context = ecoLeftyContext;
        _currentUserContext = userContext;

        _accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(ecoLeftyContext));
        _userRepository = new Lazy<IApplicationUserRepository>(() => new ApplicationUserRepository(ecoLeftyContext));
        _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(ecoLeftyContext));
        _companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(ecoLeftyContext));
        _offerRepository = new Lazy<IOfferRepository>(() => new OfferRepository(ecoLeftyContext));
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(ecoLeftyContext));
        _purchaseRepository = new Lazy<IPurchaseRepository>(() => new PurchaseRepository(ecoLeftyContext));
        _auditLogRepository = new Lazy<IAuditLogRepository>(() => new AuditLogRepository(ecoLeftyContext));
        _refreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => new RefreshTokenRepository(ecoLeftyContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken token, bool softDeleteEnabled = true)
    {
        if (softDeleteEnabled)
        {
            var now = DateTime.UtcNow;
            SoftDeleteHelper.ProcessChanges(_context, _currentUserContext, now);
        }

        return await _context.SaveChangesAsync(token);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token)
    {
        return await _context.Database.BeginTransactionAsync(token);
    }

    public IAccountRepository Accounts => _accountRepository.Value;
    public IApplicationUserRepository Users => _userRepository.Value;
    public ICategoryRepository Categories => _categoryRepository.Value;
    public ICompanyRepository Companies => _companyRepository.Value;
    public IOfferRepository Offers => _offerRepository.Value;
    public IProductRepository Products => _productRepository.Value;
    public IPurchaseRepository Purchases => _purchaseRepository.Value;
    public IAuditLogRepository AuditLogs => _auditLogRepository.Value;
    public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository.Value;

    public ICurrentUserContext CurrentUserContext => _currentUserContext;

    public EntityEntry<T> Entry<T>(T entity) where T : class
    {
        return _context.Entry(entity);
    }

    public void Attach<T>(T entity) where T : class
    {
        _context.Attach<T>(entity);
    }

    public void Detach<T>(T entity) where T : class
    {
        _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }
}