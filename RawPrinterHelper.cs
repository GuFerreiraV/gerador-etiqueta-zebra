using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeracaoEtiquetaZebraWPF
{
    public class RawPrinterHelper
    {
        // Estrutura necessária para o Windows entender o documento
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        // Importações das funções nativas do Windows (winspool.drv)
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter ([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter (IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter (IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter (IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // Método principal que você vai chamar
        public static bool SendStringToPrinter (string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            // Quantos bytes tem a string?
            dwCount = szString.Length;

            // Copia a string para a memória não gerenciada
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);

            // Envia para o helper de bytes
            bool result = SendBytesToPrinter(szPrinterName, pBytes, dwCount);

            // Libera a memória
            Marshal.FreeCoTaskMem(pBytes);
            return result;
        }

        public static bool SendBytesToPrinter (string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;

            di.pDocName = "Etiqueta Zebra";
            di.pDataType = "RAW"; // RAW diz para o driver não alterar os dados

            // Abre a impressora
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Inicia o documento
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Inicia a página
                    if (StartPagePrinter(hPrinter))
                    {
                        // Escreve os bytes
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }

            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
                Console.WriteLine("Erro ao imprimir via USB. Código Win32: " + dwError);
            }
            return bSuccess;
        }
    }
}
