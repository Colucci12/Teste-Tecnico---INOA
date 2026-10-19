namespace MonitorCotacoes.Cotacoes;

public interface InterfaceCotacoes
{
    Task<DadosCotacao> ConsultarAsync(string ticker);
}
