using minimal_api.Dominio.Enuns;

namespace minimal_api.Dominio.ModelViews
{
    public record AdministradorModelView
    {
                
        
        public int Id { get; init; }
        public string? Email { get; init; }
        public string? Perfil { get; init; }



    }
}
