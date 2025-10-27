namespace minimal_api.Dominio.ModelViews
{
    public class Home
    {
        public string Mensagem { get; set; } = "API de Veículos está em execução!";
        public string Documentacao { get => "/swagger"; }
    }
}
