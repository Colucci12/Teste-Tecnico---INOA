using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MonitorCotacoes.Email;

public class ClienteSmtp : InterfaceEmail
{
    private readonly ConfiguracaoSmtp configuracao;

    public ClienteSmtp(ConfiguracaoSmtp configuracao)
    {
        this.configuracao = configuracao;
    }

    public async Task EnviarAsync(string assunto, string texto)
    {
        if (string.IsNullOrWhiteSpace(configuracao.Servidor))
        {
            throw new InvalidOperationException("Preencha SMTP_SERVIDOR no .env.");
        }

        if (string.IsNullOrWhiteSpace(configuracao.Usuario) || string.IsNullOrWhiteSpace(configuracao.Senha))
        {
            throw new InvalidOperationException("Preencha SMTP_USUARIO e SMTP_SENHA no .env. Para Gmail, use uma senha de app.");
        }

        MimeMessage mensagem = new MimeMessage();

        mensagem.From.Add(MailboxAddress.Parse(configuracao.Remetente));
        mensagem.To.Add(MailboxAddress.Parse(configuracao.Destinatario));

        mensagem.Subject = assunto;

        TextPart corpo = new TextPart("plain");
        corpo.Text = texto;

        mensagem.Body = corpo;

        SecureSocketOptions seguranca = SecureSocketOptions.StartTls;

        if (configuracao.UsarSslDireto)
        {
            seguranca = SecureSocketOptions.SslOnConnect;
        }

        using SmtpClient cliente = new SmtpClient();
        cliente.Timeout = 15000;

        await cliente.ConnectAsync(configuracao.Servidor, configuracao.Porta, seguranca);
        await cliente.AuthenticateAsync(configuracao.Usuario, configuracao.Senha);

        await cliente.SendAsync(mensagem);
        await cliente.DisconnectAsync(true);
    }
}
