using MonitorCotacoes.Email;

namespace MonitorCotacoes.Aplicacao;

public static class EnvioEmail
{
    public static InterfaceEmail CriarCliente()
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

        InterfaceEmail cliente = new ClienteSmtp(configuracao);

        return cliente;
    }
}
