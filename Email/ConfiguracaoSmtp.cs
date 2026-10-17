namespace AprendendoCsharp.Email;

public record ConfiguracaoSmtp(
    string Servidor,
    int Porta,
    bool UsarSslDireto,
    string Usuario,
    string Senha,
    string Remetente,
    string Destinatario
);
