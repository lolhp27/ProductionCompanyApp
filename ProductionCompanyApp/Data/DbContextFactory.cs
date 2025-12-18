using Microsoft.EntityFrameworkCore;

namespace ProductionCompanyApp.Data
{
    public static class DbContextFactory
    {
        // SQL Server Authentication (логин/пароль)
        private const string ConnectionString =
            "Server=192.168.10.151;Database=355k_Kurbanov_prac;User Id=wsr-1;Password=$cYm*kL$Ny5QP#;TrustServerCertificate=True;";

        public static AppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}