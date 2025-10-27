using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos
{
    public class VeiculoServico : IVeiculoServico
    {
        private readonly DbContexto _contexto;
        public VeiculoServico(DbContexto contexto)
        {
           _contexto = contexto;
        }

        public void AdicionarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
           
        }

        public void AtualizarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo); 
            _contexto.SaveChanges();

        }

        public Veiculo? BuscarVeiculoPorId(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();

        }

        public List<Veiculo> ObterTodosVeiculos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            
            _contexto.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var query = _contexto.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
            }
            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }
            int pageSize = 10;
            int? skip = (pagina - 1) * pageSize;
            return query.Skip((int)skip).Take(pageSize).ToList();
        }

        public void RemoverVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }
    }
}
