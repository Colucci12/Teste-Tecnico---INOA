using MonitorCotacoes.Aplicacao;
using MonitorCotacoes.Entrada;

DadosEntrada? parametros = ConsoleInput.Ler(args);

if (parametros == null)
{
    return;
}

await ExecutarMonitoramento.ExecutarAsync(parametros);
