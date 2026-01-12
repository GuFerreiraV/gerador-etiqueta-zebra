# Gerador de Etiqueta Zebra (WPF)

Descrição
--------
Aplicativo desktop WPF para geração e envio de etiquetas para impressoras Zebra (usando ZPL). A interface permite montar o conteúdo da etiqueta e enviar o ZPL diretamente para a impressora via envio bruto (raw) para a fila de impressão do Windows.

Pré-requisitos
-------------
- Windows (aplicação WPF)
- Visual Studio 2017/2019/2022 (ou equivalente) com suporte a projetos WPF
- .NET Framework compatível (abrir o `.csproj` para confirmar a versão alvo)
- Impressora Zebra configurada no Windows (nome da impressora visível em "Dispositivos e Impressoras")

Instalação / Build
------------------
1. Clone o repositório:
   - git clone https://github.com/GuFerreiraV/gerador-etiqueta-zebra.git
2. Abra o arquivo de projeto no Visual Studio:
   - Abra `GeracaoEtiquetaZebraWPF.csproj` (ou a solution se existir).
3. Restaure dependências e compile:
   - Build -> Rebuild Solution
4. Execute:
   - Start (F5) para executar a aplicação no modo de desenvolvimento.
5. Publicação (opcional):
   - Use as opções de publicação do Visual Studio (Publish) ou crie um instalador apropriado.

Uso
---
- Ao abrir a aplicação você verá a janela principal com campos para montar a etiqueta (texto, possíveis campos para código de barras, tamanho, etc.) e botões para:
  - Visualizar (preview) — mostra como ficará a etiqueta (se implementado).
  - Imprimir — envia o ZPL gerado para a impressora Zebra configurada.
- Configure a impressora padrão ou selecione o nome da impressora nas configurações da aplicação (se houver).

Documentação das funções principais (resumo)
--------------------------------------------
Abaixo está uma documentação resumida das funções/rotinas que tipicamente aparecem em projetos desse tipo. Ajuste os nomes conforme os métodos do seu código.

1) RawPrinterHelper (RawPrinterHelper.cs)
   - Objetivo: enviar dados ZPL "crus" (raw) diretamente para a fila de impressão do Windows — necessário para que a impressora interprete ZPL sem alteração.
   - Funções típicas:
     - bool SendStringToPrinter(string printerName, string data)
       - Envia uma string (ZPL) para a impressora especificada.
       - Retorna `true` se o envio ocorrer sem erros.
       - Uso: constroi o ZPL como string e chama esta função com o nome da impressora.
     - bool SendBytesToPrinter(string printerName, IntPtr pBytes, int dwCount)
       - Envia um buffer de bytes para a impressora (quando já existir conversão).
     - Métodos de interoperabilidade com Win32:
       - OpenPrinter / ClosePrinter
       - StartDocPrinter / EndDocPrinter
       - StartPagePrinter / EndPagePrinter
       - WritePrinter
   - Observações:
     - Requer permissões para enviar diretamente para a fila de impressão.
     - Evitar enviar textos com codificação errada; ZPL geralmente usa ASCII/UTF-8 dependendo da versão da impressora.

2) MainWindow / UI (MainWindow.xaml.cs)
   - Objetivo: montar a interface de usuário, capturar os dados da etiqueta, montar o ZPL e disparar o envio para impressão.
   - Funções típicas:
     - void BtnPrint_Click(object sender, EventArgs e)
       - Evento do botão "Imprimir": coleta os valores dos campos, gera o ZPL e chama `RawPrinterHelper.SendStringToPrinter`.
     - string GenerateZpl(string texto, int largura, int altura, ... )
       - Constrói o ZPL da etiqueta a partir dos campos preenchidos (texto, fontes, códigos de barras, posicionamento).
       - Exemplo simples de ZPL gerado:
         ^XA
         ^FO50,50^A0N,30,30^FDTexto Demo^FS
         ^BY2,3,50^FO50,100^BCN,50,Y,N,N^FD123456789012^FS
         ^XZ
     - void BtnPreview_Click(...)
       - Gera uma visualização do ZPL (pode abrir uma janela de preview ou exportar para imagem/pdf).

Configurações importantes
-------------------------
- Nome da impressora: usar o nome exato conforme aparece em "Dispositivos e Impressoras".
- Formato da etiqueta: tamanho físico (mm) e densidade (dpi) devem corresponder à configuração da impressora/ZPL.
- Codificação: verifique a codificação dos caracteres (acentos podem precisar de fontes/ajustes específicos em ZPL).

Soluções de problemas comuns
----------------------------
- Impressora imprime texto em vez de interpretar ZPL:
  - Verifique se o driver instalado não está modificando os comandos; usar driver genérico ou enviar como "raw".
  - Use `RawPrinterHelper` para envio direto.
- Etiqueta cortada ou truncada:
  - Ajuste os parâmetros de tamanho no ZPL (field origin, feed, label length).
- Acentos/Caractères especiais:
  - Verifique suporte de fontes e setings da impressora; considere transliterar ou usar imagens.
