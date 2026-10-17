using AprendendoCsharp.Aplicacao;
using AprendendoCsharp.Entrada;
using AprendendoCsharp.Cotacoes;

DadosEntrada? parametros = ConsoleInput.Ler(args);

if (parametros == null)
{
    return;
}

DadosCotacao? cotacao = await ConsultaApi.ExecutarAsync(parametros.Ativo);

if (cotacao == null)
{
    return;
}

await EnvioEmail.ExecutarAsync(cotacao);
