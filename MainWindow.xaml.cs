using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeracaoEtiquetaZebraWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent();
        }        

        public void GerarEtiquetaZebra (string tag, string nomeImpressora)
        {
            // Validação de null e tamanho
            if (string.IsNullOrWhiteSpace(tag) || tag.Length < 1) return;

            // Lógica 
            string ultimoChar = tag.Substring(tag.Length - 1);

            // Montagem ZPL
            string zpl = $@"
                        ^XA
                        ^CI28
                        ^FO110,50^BQN,2,6^FDQA,{tag}^FS
                        ^FO0,250^FB600,1,0,C^A0N,25,25^FD{tag}^FS
                        ^FO250,60^FB400,1,0,C^A0N,150,150,C^FD{ultimoChar}^FS
                        ^XZ";

            // Enviar p/ impressora
            EnviarParaImpressora(zpl, nomeImpressora);
        }

        public void EnviarParaImpressora (string zpl, string printerName)
        {
            try
            {
                lblStatus.Text = $"Enviando para: {printerName}";

                // classe aux 
                bool success = RawPrinterHelper.SendStringToPrinter(printerName, zpl);

                if (success)
                {
                    lblStatus.Text = "DATA SENT: O Windows recebeu o comando com sucesso.";
                }
                else
                {
                    lblStatus.Text = "FALHA: Não foi possível enviar para o spooler.";
                    lblStatus.Foreground = Brushes.Red;
                }
            }
            catch (Exception e)
            {
                lblStatus.Text = $"Erro: {e.Message}";
                lblStatus.Foreground = Brushes.Red;

            }
        }

        private void txtScanner_KeyDown (object sender, KeyEventArgs e)
        {
            // O scanner sempre manda um "Enter" no final da leitura.
            // Nós só processamos se a tecla apertada for Enter.
            if (e.Key == Key.Enter)
            {
                string leitura = txtScanner.Text;

                if(leitura.ToUpper() == "FIM") this.Close();

                // 1. Validações (igual ao seu Console)
                if (leitura.Length < 5)
                {
                    lblStatus.Text = "ERRO: Leitura muito curta!";
                    lblStatus.Foreground = Brushes.Red;

                    // Limpa para tentar de novo
                    txtScanner.Clear();
                    return;
                }



                // 2. Extração dos 5 dígitos
                string lastDigits = leitura.Substring(leitura.Length - 5);
                string mesAno = txtMesAno.Text; // Pega o valor do campo na tela

                if(mesAno.Length > 4) 
                {
                    lblStatus.Text = "ERRO: Máximo 4 caracteres!";
                    lblStatus.Foreground = Brushes.Red;
                    return ;
                }

                // 3. Montagem da TAG
                const string PREFIXO_INI = "1075";
                const string PREFIXO_MEI = "1169400775R2A0600776R1A01"; // Seu código atualizado

                string tagFinal = $"{PREFIXO_INI}{mesAno}{PREFIXO_MEI}{lastDigits}";
                string nomeImpressora = "ZDesigner GX430t (Copiar 3)"; // Colocar nome de sua impressora

                // 4. Feedback Visual
                lblStatus.Text = $"Última Gerada: {tagFinal}\nStatus: Enviado para impressora.";
                lblStatus.Foreground = Brushes.Green;

                // 5. Enviar para Impressão
                GerarEtiquetaZebra(tagFinal, nomeImpressora);

                // 6. Limpar e Manter o Foco para a próxima leitura
                txtScanner.Clear();
                txtScanner.Focus();
            }
        }

    }
}