using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
           _contexto = contexto;
        }

        public Administrador? AdicionarAdministrador(Administrador administrador)
        {
                _contexto.Set<Administrador>().Add(administrador);
                _contexto.SaveChanges();

            return administrador;
        }

        public Administrador? BuscarAdministradorPorId(int id)
        {
           
            return _contexto.Set<Administrador>().Where(a => a.Id == id).FirstOrDefault();
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var administrador = _contexto.Set<Administrador>()
                .FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
            return administrador;

        }


        public List<Administrador> ObterTodosAdministradores(int? pagina = 49, string? email = null, string? senha = null, string? perfil = null)
        {
            _contexto.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var query = _contexto.Administradores.AsQueryable();
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(a => a.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(senha))
            {
                query = query.Where(a => a.Senha.Contains(senha));
            }

            if (!string.IsNullOrEmpty(perfil))
            {
                query = query.Where(a => a.Perfil.Contains(perfil));
            }
            int pageSize = 10;
            int? skip = (pagina - 1) * pageSize;
            return query.Skip((int)skip).Take(pageSize).ToList();
        }
    }
}
