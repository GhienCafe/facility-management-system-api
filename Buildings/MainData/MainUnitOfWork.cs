namespace MainData;

public class MainUnitOfWork : IDisposable
{
    private readonly DatabaseContext _context;

    public MainUnitOfWork(DatabaseContext context)
    {
        _context = context;
    }


    public void Dispose()
    {
    }
}