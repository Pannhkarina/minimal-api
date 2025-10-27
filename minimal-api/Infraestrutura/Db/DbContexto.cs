using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {

        //private readonly IConfiguration _configurationAppSettings;

        public DbContexto(DbContextOptions<DbContexto> options)
            : base(options)
        {
            //_configurationAppSettings = configurationAppSettings;
        }

        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;



    }
}
