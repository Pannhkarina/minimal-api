using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IVeiculoServico
    {

        List<Veiculo> ObterTodosVeiculos(int? pagina = '1', string? nome = null, string? marca = null);
        Veiculo? BuscarVeiculoPorId(int id);
        void AdicionarVeiculo(Veiculo veiculo);
        void AtualizarVeiculo(Veiculo veiculo);
        void RemoverVeiculo(Veiculo veiculo);


    }
}
