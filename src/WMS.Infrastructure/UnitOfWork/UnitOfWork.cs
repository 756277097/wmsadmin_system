using WMS.Core.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.UnitOfWork;

/// <summary>
/// 工作单元实现
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly WmsDbContext _context;

    public UnitOfWork(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

