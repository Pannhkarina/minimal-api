using minimal_api.Dominio.Enuns;

namespace minimal_api.Dominio.ModelViews
{
    public record AdministradorLogado
    {
                
        
       
        public string? Email { get; init; }
        public string? Perfil { get; init; }

        public string? Token { get; init; }



    }
}
