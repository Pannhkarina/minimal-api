using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);

        Administrador? AdicionarAdministrador(Administrador Adminstrador);

        List<Administrador> ObterTodosAdministradores(int? pagina = '1', string? email = null, string? senha = null, string? perfil = null);

        Administrador? BuscarAdministradorPorId(int id);



    }
}
