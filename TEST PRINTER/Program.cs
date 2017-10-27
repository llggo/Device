using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Drawing.Printing;
using System.Threading;
using System.Printing;
using System.Runtime.InteropServices;

namespace TEST_PRINTER
{
    class Program
    {
        static void Main(string[] args)
        {
            PrinterSettings ps = new PrinterSettings();
            Console.WriteLine(ps.PrinterName);
            var queue = new LocalPrintServer().GetPrintQueue(ps.PrinterName);

            while (true)
            {
                
                

                var queueStatus = queue.QueueStatus;
                //var jobStatus = queue.GetPrintJobInfoCollection().FirstOrDefault().JobStatus;

                Console.WriteLine(GetPrinterInfo(ps.PrinterName).Value.Status);
                //Console.WriteLine(jobStatus);




                Thread.Sleep(1000);

                
                
            }
        }


        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int OpenPrinter(string pPrinterName, out IntPtr phPrinter, ref PRINTER_DEFAULTS pDefault);

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetPrinter(IntPtr hPrinter, Int32 dwLevel, IntPtr pPrinter, Int32 dwBuf, out Int32 dwNeeded);

        [DllImport("winspool.drv", SetLastError = true)]
        public static extern int ClosePrinter(IntPtr hPrinter);

        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_DEFAULTS
        {
            public IntPtr pDatatype;
            public IntPtr pDevMode;
            public int DesiredAccess;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pServerName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrinterName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pShareName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPortName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverName;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pLocation;

            public IntPtr pDevMode;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pSepFile;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrintProcessor;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDatatype;

            [MarshalAs(UnmanagedType.LPTStr)]
            public string pParameters;

            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }

        public static PRINTER_INFO_2? GetPrinterInfo(String printerName)
        {
            IntPtr pHandle;
            PRINTER_DEFAULTS defaults = new PRINTER_DEFAULTS();
            PRINTER_INFO_2? Info2 = null;

            OpenPrinter(printerName, out pHandle, ref defaults);

            Int32 cbNeeded = 0;

            bool bRet = GetPrinter(pHandle, 2, IntPtr.Zero, 0, out cbNeeded);

            if (cbNeeded > 0)
            {
                IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);

                bRet = GetPrinter(pHandle, 2, pAddr, cbNeeded, out cbNeeded);

                if (bRet)
                {
                    Info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));
                }

                Marshal.FreeHGlobal(pAddr);
            }

            ClosePrinter(pHandle);

            return Info2;
        }
    
    }
}
