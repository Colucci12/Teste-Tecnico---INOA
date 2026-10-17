using AprendendoCsharp.Entrada;

namespace AprendendoCsharp.Monitoramento;

public static class RegraAlerta
{
    public static TipoAlerta Avaliar(decimal preco, DadosEntrada parametros)
    {
        if (preco < parametros.PrecoCompra)
        {
            return TipoAlerta.Compra;
        }

        if (preco > parametros.PrecoVenda)
        {
            return TipoAlerta.Venda;
        }

        return TipoAlerta.Nenhum;
    }
}
