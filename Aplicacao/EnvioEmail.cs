using AprendendoCsharp.Cotacoes;
using AprendendoCsharp.Email;

namespace AprendendoCsharp.Aplicacao;

public static class EnvioEmail
{
    public static async Task ExecutarAsync(DadosCotacao cotacao)
    {
        try
        {
            string servidor = Environment.GetEnvironmentVariable("SMTP_SERVIDOR") ?? "";
            string usuario = Environment.GetEnvironmentVariable("SMTP_USUARIO") ?? "";
            string senha = Environment.GetEnvironmentVariable("SMTP_SENHA") ?? "";

            string remetente = Environment.GetEnvironmentVariable("EMAIL_REMETENTE") ?? "";
            string destinatario = Environment.GetEnvironmentVariable("EMAIL_DESTINATARIO") ?? "";

            string? portaTexto = Environment.GetEnvironmentVariable("SMTP_PORTA");
            bool portaValida = int.TryParse(portaTexto, out int porta);

            if (!portaValida)
            {
                throw new InvalidOperationException("Preencha SMTP_PORTA no .env com um número inteiro, por exemplo: 587.");
            }

            string sslTexto = Environment.GetEnvironmentVariable("SMTP_SSL_DIRETO") ?? "false";
            bool sslValido = bool.TryParse(sslTexto, out bool usarSslDireto);

            if (!sslValido)
            {
                throw new InvalidOperationException("SMTP_SSL_DIRETO deve ser true ou false no .env.");
            }

            ConfiguracaoSmtp configuracao = new ConfiguracaoSmtp(
                servidor,
                porta,
                usarSslDireto,
                usuario,
                senha,
                remetente,
                destinatario
            );

            string assunto = "Cotação de " + cotacao.Ativo;

            string texto = "Ativo: " + cotacao.Ativo + Environment.NewLine;
            texto += "Preço: R$ " + cotacao.Preco.ToString("F2") + Environment.NewLine;
            texto += "Horário da cotação: " + cotacao.Horario;

            ClienteSmtp cliente = new ClienteSmtp(configuracao);

            await cliente.EnviarAsync(assunto, texto);

            Console.WriteLine("E-mail enviado ao servidor SMTP com sucesso.");
        }
        catch (MailKit.Security.AuthenticationException)
        {
            Console.Error.WriteLine("O servidor SMTP recusou a autenticação. Confira SMTP_USUARIO e SMTP_SENHA no .env; no Gmail, use uma senha de app.");
            Environment.ExitCode = 1;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Não foi possível enviar o e-mail: " + exception.Message);
            Console.Error.WriteLine("Verifique as configurações SMTP no .env.");
            Environment.ExitCode = 1;
        }
    }
}
