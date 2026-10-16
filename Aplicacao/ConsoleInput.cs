using AprendendoCsharp.Entrada;

namespace AprendendoCsharp.Aplicacao;

public static class ConsoleInput
{
    public static DadosEntrada? Ler(string[] args)
    {
        DadosEntrada? parametros = null;
        string? erro = null;

        bool entradaValida = LeitorDeArgumentos.TryParse(args, out parametros, out erro);

        if (!entradaValida || parametros == null)
        {
            Console.Error.WriteLine(erro);
            return null;
        }

        Console.WriteLine(parametros);

        return parametros;
    }
}
