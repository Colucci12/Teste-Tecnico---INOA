namespace AprendendoCsharp.Email;

public interface InterfaceEmail
{
    Task EnviarAsync(string assunto, string texto);
}
