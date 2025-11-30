using ISession = NHibernate.ISession;

namespace DotnetDemo.Middleware;

public class NHibernateSessionMiddleware
{
    private readonly RequestDelegate _next;

    public NHibernateSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISession session)
    {
        using var transaction = session.BeginTransaction();
        try
        {
            await _next(context);

            if (transaction.IsActive)
            {
                await transaction.CommitAsync();
            }
        }
        catch
        {
            if (transaction.IsActive)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
        finally
        {
            session.Clear();
        }
    }
}

