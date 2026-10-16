namespace AprendendoCsharp.Cotacoes;

public record DadosCotacao(
    string Ativo,
    decimal Preco,
    DateTimeOffset Horario
);
