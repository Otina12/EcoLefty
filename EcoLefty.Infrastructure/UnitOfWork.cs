using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Contracts.Repositories.Common;
using EcoLefty.Infrastructure.Repositories;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EcoLefty.Infrastructure;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly EcoLeftyDbContext _context;

    private readonly Lazy<IApplicationUserRepository> _userRepository;
    private readonly Lazy<ICategoryRepository> _categoryRepository;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly Lazy<IOfferRepository> _offerRepository;
    private readonly Lazy<IProductRepository> _productRepository;
    private readonly Lazy<ISoftDeletableRepository> _softDeletableRepository;

    public UnitOfWork(EcoLeftyDbContext ecoLeftyContext)
    {
        _context = ecoLeftyContext;

        _userRepository = new Lazy<IApplicationUserRepository>(() => new ApplicationUserRepository(ecoLeftyContext));
        _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(ecoLeftyContext));
        _companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(ecoLeftyContext));
        _offerRepository = new Lazy<IOfferRepository>(() => new OfferRepository(ecoLeftyContext));
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(ecoLeftyContext));
        _softDeletableRepository = new Lazy<ISoftDeletableRepository>(() => new SoftDeletableRepository(ecoLeftyContext));
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public IApplicationUserRepository Users => _userRepository.Value;
    public ICategoryRepository Categories => _categoryRepository.Value;
    public ICompanyRepository Companies => _companyRepository.Value;
    public IOfferRepository Offers => _offerRepository.Value;
    public IProductRepository Products => _productRepository.Value;
    public ISoftDeletableRepository SoftDeleteRepository => _softDeletableRepository.Value;

    public EntityEntry<T> Entry<T>(T entity) where T : class
    {
        return _context.Entry(entity);
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