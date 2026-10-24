# MyMoney

Mod de **Stardew Valley** (SMAPI) para enviar dinheiro a outro jogador em
multiplayer com carteiras separadas — de qualquer lugar do mapa.

O jogo base já permite transferir dinheiro: basta ir até a casa do prefeito
Lewis e usar o livro-caixa da cidade. O mod não inventa uma função nova, ele
tira a caminhada: você paga de onde estiver, no meio da colheita ou dentro da
mina.

> Requer um save **multiplayer com dinheiro separado**. Em save de carteira
> compartilhada não existe transferência: todos já gastam do mesmo dinheiro, e
> o mod avisa isso em vez de fingir que transferiu.

## Como usar

### Pela tela do jogo

Clique na **caixa de dinheiro**, no canto superior direito. Abre a lista de
jogadores online; clique em um deles e escolha o valor na barra deslizante.
Funciona com mouse e com controle.

### Pelo console do SMAPI

Na janela preta do SMAPI (a que abre junto com o jogo), digite:

```
player_pay <nome do jogador> <valor>
player_list
```

Exemplos:

```
player_pay Bob 5000
player_pay Maria Clara 5000     # nomes com espaço funcionam
player_list                     # mostra os nomes exatos de quem está online
```

O **último** argumento é sempre o valor, então não precisa de aspas para nomes
compostos. Se não souber o nome exato de alguém, rode `player_list` primeiro: o
nome do fazendeiro pode ser diferente do apelido que aparece no chat.

## Compilando

Precisa do .NET SDK 6.0+ e do Stardew Valley instalado (o
[ModBuildConfig](https://github.com/Pathoschild/Stardew.ModBuildConfig) acha o
jogo sozinho e copia o build para a pasta `Mods`).

```
dotnet build mymoney/MyMoney/MyMoney.csproj
```

## Estrutura

O mod é dividido por responsabilidade: nenhuma tela mexe em dinheiro, e a regra
de negócio não escreve mensagem para o jogador.

| Pasta | Papel |
|---|---|
| `ModEntry.cs` | Monta as peças e liga os eventos. Nada de regra. |
| `Core/` | Tipos de domínio: resultado da transferência, validação de argumentos, textos (`Translations`, `MessageFormatter`). |
| `Services/` | `WalletService` é o **único** lugar que movimenta dinheiro; `PlayerLookup` é o único que resolve nomes. |
| `Network/` | O farmhand pede, o host credita (`CreditRequest`) e responde: aviso a quem recebeu (`CreditReceipt`) ou estorno a quem pagou (`CreditRefund`). Evita escrever na carteira alheia de outra máquina. |
| `Commands/` | Comandos de console, um por arquivo, registrados pelo `CommandRegistry`. |
| `UI/` | Clique na caixa de dinheiro, lista de jogadores e seletor de valor nativo do jogo. |
| `i18n/` | Textos em inglês (`default.json`) e português (`pt-BR.json`). |

Para adicionar um comando novo, crie a classe em `Commands/` implementando
`IConsoleCommand` e inclua na lista do `ModEntry` — o `Entry` não cresce.

## Histórico

**Não lançado**

- Quem recebe dinheiro é avisado na tela. Antes o crédito chegava em silêncio:
  o número subia sozinho e não se sabia de quem veio.
- Dinheiro não some mais quando o host recusa o crédito. Quem paga de um
  farmhand debita antes de pedir; se o destino desconectar nesse intervalo, o
  valor volta para a carteira com um aviso, em vez de deixar de existir.

**2.0.0**

- Tela no jogo: clicar na caixa de dinheiro abre o envio (antes só havia console).
- Nomes com espaço funcionam (`player_pay Maria Clara 5000`).
- Novo comando `player_list`, para achar o nome exato de quem está online.
- Corrigidos travamentos: comando sem argumentos, valor não numérico.
- Valor negativo não transfere mais ao contrário (roubando do alvo).
- Recusa saves de carteira compartilhada, onde a transferência fazia dinheiro
  desaparecer.
- Não aceita mais pagar a si mesmo, nem pagar para quem está offline.
- Busca de nome sem diferenciar maiúsculas, com aviso quando o nome é ambíguo.
- Em multiplayer, o crédito passa pelo host em vez de ser escrito pelo farmhand.
- Textos em inglês e português.
- Atualizado para Stardew Valley 1.6 / SMAPI 4 (net6.0).

**1.0.0** — comando `player_pay` no console.
