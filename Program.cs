using AprendendoCsharp.Entrada;

DadosEntrada? parametros = null;
string? erro = null;

bool entradaValida = LeitorDeArgumentos.TryParse(args, out parametros, out erro);

if (!entradaValida)
{
    Console.Error.WriteLine(erro);
    return;
}

Console.WriteLine(parametros);
