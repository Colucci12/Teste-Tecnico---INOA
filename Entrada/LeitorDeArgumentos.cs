using System.Globalization;

namespace AprendendoCsharp.Entrada;

public static class LeitorDeArgumentos
{
    public static bool TryParse(
        string[] args,
        out DadosEntrada? parametros,
        out string? erro)
    {
        if (args.Length != 3)
        {
            erro = "Os argumentos estão errados, o esperado é: [TAG DO ATIVO] [PREÇO VENDA] [PREÇO COMPRA]";
            parametros = null;
            return false;
        }

        string ticker = args[0].Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(ticker))
        {
            erro = "Informe o código do ativo, por exemplo: XPML11";
            parametros = null;
            return false;
        }

        bool isVendaValida = decimal.TryParse(
            args[1],
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out decimal valorVendaParseado
        );

        bool isCompraValida = decimal.TryParse(
            args[2],
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out decimal valorCompraParseado
        );

        if (!isVendaValida || !isCompraValida)
        {
            erro = "Preços inválidos! Use ponto decimal, exemplo: 37.90";
            parametros = null;
            return false;
        }

        if (valorCompraParseado >= valorVendaParseado)
        {
            erro = "Preços inválidos! O preço de venda deve ser maior que o preço de compra";
            parametros = null;
            return false;
        }

        erro = null;
        parametros = new DadosEntrada(ticker, valorVendaParseado, valorCompraParseado);
        return true;
    }
}
