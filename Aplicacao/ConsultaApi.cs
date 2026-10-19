using MonitorCotacoes.Cotacoes;

namespace MonitorCotacoes.Aplicacao;

public static class ConsultaApi
{
    public static InterfaceCotacoes CriarCliente(HttpClient httpClient)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(15);

        InterfaceCotacoes cliente = new ClienteBrapi(httpClient);

        return cliente;
    }
}
