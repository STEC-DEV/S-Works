
using Dapper;
using FamTec.Server.Databases;
using FamTec.Shared.Model;
using System.Data;

namespace FamTec.Server.Repository.DapperTemp
{
    public class DapperTempRepository : IDapperTempRepository
    {
        private readonly WorksContext context; // EF Core
        private readonly IDbConnection connection; // Dapper

        public DapperTempRepository(WorksContext _context, IDbConnection _connection)
        {
            this.context = _context;
            this.connection = _connection;
        }

        public async Task SelectUser()
        {
            string sql = "SELECT * FROM users_tb WHERE Name = @Name";

            var result = await connection.QueryAsync(sql, new { Name = "김용우" });

            //var sql = "SELECT * from users_tb";

            Console.WriteLine("asdfasdf");
            
        }
    }
}
