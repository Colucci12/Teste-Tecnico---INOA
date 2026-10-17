using AprendendoCsharp.Cotacoes;
using AprendendoCsharp.Email;
using AprendendoCsharp.Entrada;
using AprendendoCsharp.Monitoramento;

namespace AprendendoCsharp.Aplicacao;

public static class ExecutarMonitoramento
{
    public static async Task ExecutarAsync(DadosEntrada parametros)
    {
        try
        {
            if (File.Exists(".env"))
            {
                DotNetEnv.Env.Load();
            }

            string intervaloTexto = Environment.GetEnvironmentVariable("INTERVALO_CONSULTA_SEGUNDOS") ?? "300";
            bool intervaloValido = int.TryParse(intervaloTexto, out int intervaloSegundos);

            if (!intervaloValido || intervaloSegundos <= 0)
            {
                throw new InvalidOperationException("INTERVALO_CONSULTA_SEGUNDOS deve ser um número inteiro maior que zero.");
            }

            TimeSpan intervalo = TimeSpan.FromSeconds(intervaloSegundos);

            using HttpClient httpClient = new HttpClient();

            InterfaceCotacoes clienteCotacoes = ConsultaApi.CriarCliente(httpClient);
            InterfaceEmail clienteEmail = EnvioEmail.CriarCliente();

            MonitorAtivo monitor = new MonitorAtivo(clienteCotacoes, clienteEmail);

            Console.WriteLine("Monitorando " + parametros.Ativo + ". Intervalo entre consultas: " + intervaloSegundos + " segundos.");

            await monitor.ExecutarAsync(parametros, intervalo);
        }
        catch (HttpRequestException exception)
        {
            Console.Error.WriteLine("Falha na consulta: " + exception.Message);
            Environment.ExitCode = 1;
        }
        catch (TaskCanceledException)
        {
            Console.Error.WriteLine("A operação demorou demais. O monitoramento foi encerrado.");
            Environment.ExitCode = 1;
        }
        catch (System.Text.Json.JsonException)
        {
            Console.Error.WriteLine("A API retornou um JSON inválido. O monitoramento foi encerrado.");
            Environment.ExitCode = 1;
        }
        catch (MailKit.Security.AuthenticationException)
        {
            Console.Error.WriteLine("O servidor SMTP recusou a autenticação. Confira SMTP_USUARIO e SMTP_SENHA no .env; no Gmail, use uma senha de app.");
            Environment.ExitCode = 1;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("O monitoramento foi encerrado com erro: " + exception.Message);
            Environment.ExitCode = 1;
        }
    }
}
