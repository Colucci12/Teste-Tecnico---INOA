using MonitorCotacoes.Cotacoes;
using MonitorCotacoes.Email;
using MonitorCotacoes.Entrada;

namespace MonitorCotacoes.Monitoramento;

public class MonitorAtivo
{
    private readonly InterfaceCotacoes clienteCotacoes;
    private readonly InterfaceEmail clienteEmail;

    public MonitorAtivo(InterfaceCotacoes clienteCotacoes, InterfaceEmail clienteEmail)
    {
        this.clienteCotacoes = clienteCotacoes;
        this.clienteEmail = clienteEmail;
    }

    public async Task ExecutarAsync(DadosEntrada parametros, TimeSpan intervalo)
    {
        while (true)
        {
            DadosCotacao cotacao = await clienteCotacoes.ConsultarAsync(parametros.Ativo);

            Console.WriteLine(cotacao);

            TipoAlerta alerta = RegraAlerta.Avaliar(cotacao.Preco, parametros);

            if (alerta == TipoAlerta.Nenhum)
            {
                Console.WriteLine("Cotação dentro dos limites. Aguardando a próxima consulta.");

                await Task.Delay(intervalo);

                continue;
            }

            string acao = "venda";
            decimal limite = parametros.PrecoVenda;

            if (alerta == TipoAlerta.Compra)
            {
                acao = "compra";
                limite = parametros.PrecoCompra;
            }

            string assunto = "Alerta de " + acao + ": " + cotacao.Ativo;

            string texto = "Ativo: " + cotacao.Ativo + Environment.NewLine;
            texto += "Cotação: R$ " + cotacao.Preco.ToString("F2") + Environment.NewLine;
            texto += "Limite de " + acao + ": R$ " + limite.ToString("F2") + Environment.NewLine;
            texto += "Horário da cotação: " + cotacao.Horario + Environment.NewLine;
            texto += "Sugestão: efetuar a " + acao + " conforme o limite configurado.";

            await clienteEmail.EnviarAsync(assunto, texto);

            Console.WriteLine("Alerta de " + acao + " enviado ao servidor SMTP. Monitoramento encerrado.");

            return;
        }
    }
}
