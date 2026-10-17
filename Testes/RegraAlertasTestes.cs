using AprendendoCsharp.Entrada;
using AprendendoCsharp.Monitoramento;

namespace AprendendoCsharp.Testes;

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
}
