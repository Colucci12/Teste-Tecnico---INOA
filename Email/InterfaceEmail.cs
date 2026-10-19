namespace MonitorCotacoes.Email;

public interface InterfaceEmail
{
    Task EnviarAsync(string assunto, string texto);
}
