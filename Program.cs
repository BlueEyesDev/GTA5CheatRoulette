using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        //1 (case, 1st 12) = 330k
        static byte[] DoubleZero_RouletteCasino = new byte[] {
            1, 00, 00, 00, 00, 00, 00, 00,
            1, 00, 00, 00, 00, 00, 00, 00,
            1, 00, 00, 00, 00, 00, 00, 00,
            1, 00, 00, 00, 00, 00, 00, 00,
            1, 00, 00, 00, 00, 00, 00, 00,
            1, 00, 00, 00, 00, 00, 00, 00
        };
       

        static void Main(string[] args)
        {

            Mem mem = new Mem("GTA5");
            

            while (true)
            {
                long Roulette_Pointer = mem.GetPtrAddr(0x02A843C8, new int[] { 0x28, 0x8, 0x108, 0x4D0 });
                mem.Write(Roulette_Pointer, DoubleZero_RouletteCasino);
            }
            Thread.Sleep(-1);
        }
    }

    class Mem
    {

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr Handle, long Address, byte[] buffer, int Size, int BytesWritten = 0);
        [DllImport("kernel32.dll")]
        public static extern int ReadProcessMemory(IntPtr Handle, long Address, byte[] buffer, int Size, int BytesRead = 0);

        public Process Proc;
        public long BaseAddress;

        public Mem(string process)
        {
            try {
                Proc = Process.GetProcessesByName(process)[0];
                BaseAddress = Proc.MainModule.BaseAddress.ToInt64();
            }
            catch { throw new Exception(); }
        }
    
        public IntPtr GetProcHandle()
        {
            try { return Proc.Handle; } catch { return IntPtr.Zero; }
        }

        public long GetPtrAddr(long Pointer, int[] Offset = null)
        {
            byte[] Buffer = new byte[8];

            ReadProcessMemory(GetProcHandle(), BaseAddress + Pointer, Buffer, Buffer.Length);
           
            Pointer = BitConverter.ToInt64(Buffer, 0);
            if (Offset != null)
            {
                for (int x = 0; x < (Offset.Length - 1); x++)
                {

                    Pointer = BitConverter.ToInt64(Buffer, 0) + Offset[x];
                    ReadProcessMemory(GetProcHandle(), Pointer, Buffer, Buffer.Length);
                }
                Pointer = BitConverter.ToInt64(Buffer, 0) + Offset[Offset.Length - 1];
            }
            return Pointer;
        }
        public byte[] ReadBytes(long Pointer, int Length)
        {
            byte[] Buffer = new byte[Length];
            ReadProcessMemory(GetProcHandle(), Pointer, Buffer, Length);
            return Buffer;
        }
        public void Write(long Pointer, byte[] Bytes) => WriteProcessMemory(GetProcHandle(), Pointer, Bytes, Bytes.Length);
    }

}
