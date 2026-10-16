using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AprendendoCsharp.Cotacoes;

public class ClienteBrapi
{
    private readonly HttpClient httpClient;

    public ClienteBrapi(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DadosCotacao> ConsultarAsync(string ticker)
    {
        string? token = Environment.GetEnvironmentVariable("BRAPI_TOKEN");

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token não configurado. Preencha BRAPI_TOKEN no arquivo .env antes de consultar.");
        }

        string url = "https://brapi.dev/api/v2/stocks/quote?symbols="
            + Uri.EscapeDataString(ticker);

        using HttpRequestMessage requisicao = new HttpRequestMessage(HttpMethod.Get, url);

        requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using HttpResponseMessage resposta = await httpClient.SendAsync(requisicao);

        if (resposta.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new HttpRequestException("A brapi recusou o token informado. Confira BRAPI_TOKEN no arquivo .env.");
        }

        if (resposta.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException("Seu plano da brapi não permite esta consulta.");
        }

        if (resposta.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpRequestException("Nenhuma cotação encontrada para o ativo informado.");
        }

        if (resposta.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new HttpRequestException("O limite de consultas da brapi foi atingido. Tente novamente mais tarde.");
        }

        resposta.EnsureSuccessStatusCode();

        string json = await resposta.Content.ReadAsStringAsync();
        using JsonDocument documento = JsonDocument.Parse(json);

        JsonElement resultado = documento.RootElement.GetProperty("results")[0];
        JsonElement dados = resultado.GetProperty("data");

        string? ativo = resultado.GetProperty("symbol").GetString();
        decimal preco = dados.GetProperty("regularMarketPrice").GetDecimal();
        DateTimeOffset horario = dados.GetProperty("regularMarketTime").GetDateTimeOffset();

        if (ativo == null)
        {
            throw new InvalidOperationException("A API retornou uma cotação sem o código do ativo.");
        }

        return new DadosCotacao(ativo, preco, horario);
    }
}
