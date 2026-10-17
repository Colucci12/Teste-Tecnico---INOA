using AprendendoCsharp.Cotacoes;

namespace AprendendoCsharp.Aplicacao;

public static class ConsultaApi
{
    public static async Task<DadosCotacao?> ExecutarAsync(string ticker)
    {
        if (File.Exists(".env"))
        {
            DotNetEnv.Env.Load();
        }

        using HttpClient httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(15);

        InterfaceCotacoes cliente = new ClienteBrapi(httpClient);

        try
        {
            DadosCotacao cotacao = await cliente.ConsultarAsync(ticker);
            Console.WriteLine(cotacao);

            return cotacao;
        }
        catch (HttpRequestException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Environment.ExitCode = 1;
        }
        catch (TaskCanceledException)
        {
            Console.Error.WriteLine("A consulta demorou demais. Tente novamente.");
            Environment.ExitCode = 1;
        }
        catch (System.Text.Json.JsonException)
        {
            Console.Error.WriteLine("A API retornou um JSON inválido.");
            Environment.ExitCode = 1;
        }
        catch (InvalidOperationException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Environment.ExitCode = 1;
        }

        return null;
    }
}
