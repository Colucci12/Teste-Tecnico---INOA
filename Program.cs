using AprendendoCsharp.Aplicacao;
using AprendendoCsharp.Entrada;

DadosEntrada? parametros = ConsoleInput.Ler(args);

if (parametros == null)
{
    return;
}

await ExecutarMonitoramento.ExecutarAsync(parametros);
