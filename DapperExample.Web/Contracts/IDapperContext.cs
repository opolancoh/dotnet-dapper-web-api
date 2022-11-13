using System.Data;

namespace DapperExample.Web.Contracts;

public interface IDapperContext
{
    IDbConnection CreateConnection();
}