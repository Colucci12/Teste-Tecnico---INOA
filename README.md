# Monitor de Cotações - Teste Técnico Inoa

Este projeto implementa um monitor de cotações em C#/.NET para o desafio técnico da INOA. Como meu contato prévio com C# era limitado, também usei o desenvolvimento para me familiarizar com o ecossistema e seus padrões. Nesse README vou registrar como o projeto funciona, as decisões que tomei.

## Como o projeto funciona

O programa é um CLI que recebe três argumentos, nessa ordem:
```text
[código do ativo] [limite de venda] [limite de compra]
```

Por exemplo:
```terminal
dotnet run -- PETR4 22.67 22.59
```

O fluxo começa pela validação dos argumentos. Com a entrada válida, o programa busca a cotação do ativo na [brapi](https://brapi.dev/) e compara o preço retornado com os limites de compra e venda. O resultado dessa comparação define o próximo passo:

- Se o preço passar do limite de venda, envia um e-mail sugerindo vender o ativo.
- Se ficar abaixo do limite de compra, envia um e-mail sugerindo comprar o ativo.
- Enquanto o preço estiver dentro da faixa, aguarda o intervalo configurado e consulta novamente.
OBS: Isso também vale quando o preço está exatamente igual a um dos limites.

Logo que começa o programa já efetua a primeira consulta. Se o preço já estiver fora da faixa, ele pode enviar o alerta nessa primeira consulta. Após enviar um alerta (seja de compra ou de venda) com sucesso, o programa encerra. Caso ocorra um erro na consulta ou no envio, ele informa o problema no terminal e também encerra, com código de erro. Para interromper enquanto está aguardando, basta usar `Ctrl+C`.

## Decisões e aprendizados

### 1. Arquitetura e organização

Para o escopo deste desafio, escolhi uma aplicação **monolítica**. Para um programa de console com esse escopo, essa estrutura já atende aos requisitos.

Com essa estrutura em mente, comecei com um projeto de console, primeiro entendendo como receber e validar os argumentos. Conforme entraram a consulta à API, o envio de e-mail e o monitoramento, fui separando essas responsabilidades.

Uma referência foi a arquitetura hexagonal, que eu já conhecia do meu último time, em um projeto para o Bradesco. Me inspirei principalmente na separação entre as regras da aplicação e as integrações externas, usando interfaces para conectar essas partes.

Usei esses princípios como inspiração, sem buscar uma implementação completa da arquitetura hexagonal. Mantive um projeto principal organizado em pastas e, para esse escopo, não vi necessidade de acrescentar mais camadas e abstrações.

A organização ficou assim:

```text
Aplicacao/      Prepara os clientes e inicia a execução.
Entrada/        Recebe, converte e valida os argumentos.
Cotacoes/       Consulta a API e representa os dados da cotação.
Email/          Configura e realiza o envio por SMTP.
Monitoramento/  Compara os preços e coordena as consultas e os alertas.
Testes/         Projeto separado com os testes da regra de alerta.
```
Eu queria conseguir olhar o `Program.cs` e entender a sequência da aplicação. Por isso, ele ficou responsável por chamar a leitura da entrada e iniciar o monitoramento. A preparação dos clientes e o tratamento dos erros ficaram na pasta `Aplicacao`.

Separei a regra que compara os preços da parte que consulta a API e envia e-mail. Assim, consigo testar a decisão de compra ou venda passando um preço qualquer, sem depender de a bolsa estar aberta ou de uma chamada à brapi.

As interfaces de cotação e e-mail foram outra parte do aprendizado. O `MonitorAtivo` recebe esses contratos e usa os métodos que eles oferecem. Ele não precisa saber como montar uma requisição HTTP ou autenticar no SMTP. A escolha de usar `ClienteBrapi` e `ClienteSmtp` continua na preparação da aplicação.

Para guardar os dados de entrada, cotação e configuração SMTP, usei `record`. Mantive o mesmo formato nos três para deixar a leitura consistente enquanto aprendia a sintaxe.

Na consulta, usei o `HttpClient`, que já vem no .NET. No envio de e-mail, usei MailKit com SMTP e testei com Gmail. As configurações ficam no `.env`, e o repositório contém apenas o `.env.exemplo`, sem credenciais.

Sobre a consulta dos ativos, aqui tinham algumas opções que considerei:
1. Pooling - Essa opção foi a utilizada, é possivel configurar no `.env` o tempo entre uma consulta e outra. Alguns problemas dessa opção são: a consulta da cotação não vai estar 100% sincronizada, as APIs geralmente tem limites de consulta por mes + um limite de consultas em curto período de tempo.
2. WebHook - Esse é um serviço que tem em diversas APIs, basicamente o programa só cadastra na API quais os limites e espera esta mesma API retornar. Esse não foi escolhido por que julguei que iria tercerizar o core desse desafio, então não fazia sentido nesse caso. Os problemas dessa solução são: Fica dependente de um terceiro, se essa API falhar, atrasar ou algo assim, o programa fica dependente dela. Simplifica a regra mas aumenta a dependência de um serviço terceiro.
3. Websocket - Nesse serviço uma conexão aberta e os dados chegam do fornecedor através dela. Essa solução traz atualizações muito rapidamente, entretanto alguns problemas são: dependencia da qualidade do fornecedor, se ele atrasa os dados, o programa recebe atrasado da mesma forma. É um tipo de conexão que é mais complexa de se implementar e manter do que consultas como nas duas opções acima.

> **Nota**
>
> **Uso de IA:** Usei IA como apoio nas conversas sobre a organização do projeto, como alguém mais experiente acompanhando o desenvolvimento. Como ainda não domino a sintaxe de C#, pedi explicações sobre como declarar interfaces, organizar as classes e conectar essas partes.

### 2. Fluxo e regras de negócio

O limite de compra precisa ser menor que o de venda. O enunciado não exige essa validação explicitamente, mas escolhi manter os dois limites como uma faixa. Para disparar o alerta, segui o "maior que" e "menor que" do texto: se a cotação for exatamente igual a um dos limites, o programa continua aguardando.

No código do ativo, removo espaços das pontas e converto o texto para maiúsculas. Não coloquei uma regra rígida de quantidade de letras e números; a confirmação de que existe uma cotação fica com a API. Já o token precisa estar preenchido antes da consulta, e a brapi informa se ele é válido. Preferi dessa forma pois não quero criar uma regra que possivelmente limite o programa de consultar um ativo que realmente existe.

Escolhi encerrar depois do primeiro alerta enviado para evitar repetir e-mails enquanto a cotação permanece fora da faixa. Para monitorar novamente, o usuário inicia outra execução e pode informar novos limites. Para não aumentar muito o escopo do projeto essa pareceu a melhor abordagem, mais sensata. Mas com a divisão de responsabilidades no código ficaria bem fácil trocar o comportamento caso alguma outra decisão fosse tomada.

Enquanto não há alerta, o programa aguarda antes de consultar novamente. Deixei o intervalo configurável no `.env`, com padrão de 300 segundos. A atualização dos dados depende da API e do plano utilizado; fazer mais requisições não garante receber uma cotação diferente, isso é uma LIMITAÇÃO da api da BRAPI, que escolhi para esse projeto. Entretanto por conta da estrutura de interfaces, adicionar diferentes "fornecedores" (APIs) não atrapalharia o restante do programa.

Nesta versão, erros de consulta ou envio são exibidos no terminal e encerram o programa. Não implementei novas tentativas automáticas após essas falhas.

> **Nota**
>
> **Uso de IA:** Usei IA para conversar sobre as regras e suas consequências, como encerrar após um alerta ou continuar monitorando. Também pedi apoio para transformar essas decisões em código C# e explicar trechos cuja sintaxe eu ainda não conhecia. Conferi o fluxo executando e fazendo validações com teste.

## Como executar

**OBS:** Os comandos abaixo devem ser executados na raiz do projeto.

### Configuração

1. Se ainda não houver um `.env`, copie o `.env.exemplo` para `.env` e preencha as variáveis;
2. Gere uma chave de API da BRAPI e preencha `BRAPI_TOKEN`;
3. Para usar Gmail, ative a verificação em duas etapas e gere uma [senha de app](https://support.google.com/accounts/answer/185833?hl=pt-BR) para preencher `SMTP_SENHA`.


### Com o SDK do .NET

Instale o SDK indicado no `global.json` (`10.0.402`) e execute:
```powershell
dotnet run -- PETR4 22.67 22.59
```

### Com Docker Compose

Se voce quiser apenas rodar o programa:
```powershell
docker compose run --rm monitor PETR4 22.67 22.59
```

Se voce quiser gerar um executavel win x64:
```powershell
docker compose -f compose.criar-executavel.yaml run --rm publicar
```

A saída fica em `publicacao/`, com o executável e suas dependências. Essa publicação faz com que o .NET não tenha necessidade de estar instalado na máquina.
Pra rodar:
```powershell
& ".\publicacao\MonitorCotacoes.exe" PETR4 23.61 20.78
```

O `.env` é procurado na pasta atual do terminal. É só garantir que o `.env` está criado e preenchido corretamente

> **Nota**
>
> **Uso de IA:** Usei IA para entender as opções de execução e publicação, me auxiliar em entender os comandos do .NET e ajudar na criação do Docker Compose.

## Testes

Usei xUnit em um projeto separado. Mantive o escopo em três testes da `RegraAlerta`:

- Retornar compra quando o preço está abaixo do limite de compra.
- Retornar venda quando o preço está acima do limite de venda.
- Não gerar alerta quando o preço está dentro da faixa.

Para executar, com o SDK do .NET instalado:
```powershell
dotnet test ".\Testes\MonitorCotacoes.Testes.csproj"
```

Esses testes usam valores fixos e não fazem consultas à brapi nem enviam e-mails.

Também aproveitei essa parte para aprender a estrutura e a sintaxe dos testes em C#. A cobertura automatizada atual é limitada: não inclui os valores exatamente iguais aos limites, a validação dos argumentos, as falhas de integração ou o fluxo completo do monitoramento. Esses seriam pontos para ampliar em uma evolução do projeto.

> **Nota**
>
> **Uso de IA:** Usei IA para entender a configuração do xUnit no projeto, além de também me ajudar a entender algumas sintaxes dos testes, como por exemplo o significado de `[Fact]`.
