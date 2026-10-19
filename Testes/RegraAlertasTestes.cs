using MonitorCotacoes.Entrada;
using MonitorCotacoes.Monitoramento;

namespace MonitorCotacoes.Testes;

public class RegraAlertasTestes
{
    [Fact]
    public void PrecoAbaixoDoLimiteCompra()
    {
        DadosEntrada parametros = new DadosEntrada(
            "PETR4",
            25m,
            20m
        );

        decimal precoAtual = 19m;

        TipoAlerta resultado = RegraAlerta.Avaliar(precoAtual, parametros);

        Assert.Equal(TipoAlerta.Compra, resultado);
    }

    [Fact]
    public void PrecoAcimaDoLimiteVenda()
    {
        DadosEntrada parametros = new DadosEntrada(
            "PETR4",
            25m,
            20m
        );

        decimal precoAtual = 26m;

        TipoAlerta resultado = RegraAlerta.Avaliar(precoAtual, parametros);

        Assert.Equal(TipoAlerta.Venda, resultado);
    }

    [Fact]
    public void PrecoDentroDosLimites()
    {
        DadosEntrada parametros = new DadosEntrada(
            "PETR4",
            25m,
            20m
        );

        decimal precoAtual = 23m;

        TipoAlerta resultado = RegraAlerta.Avaliar(precoAtual, parametros);

        Assert.Equal(TipoAlerta.Nenhum, resultado);
    }
}
