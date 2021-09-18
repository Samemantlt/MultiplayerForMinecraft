using System.Diagnostics;
using System.Linq;
using System.Net;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;

namespace MultiplayerForMinecraftCommon
{
    public static class MinecraftListenerDetector
    {
        private enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint dwState;
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public uint dwRemoteAddr;
            public uint dwRemotePort;
            public int dwOwningPid;
        }


        private const uint ERROR_INSUFFICIENT_BUFFER = 122;
        private const uint NO_ERROR = 0;
        private const uint AF_INET = 2;
        private const uint AF_INET6 = 23;



        [DllImport("Ws2_32.dll")]
        private static extern ushort htons(ushort a);

        [DllImport("Iphlpapi.dll", SetLastError = true, ExactSpelling = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int pdwSize,
            [MarshalAs(UnmanagedType.Bool)] bool bOrder,
            uint ulAf,
            TCP_TABLE_CLASS TableClass,
            uint Reserved
            );

        private static List<Connection> GetListeners()
        {
            IntPtr ptr = IntPtr.Zero;
            int buffSize = 0;
            uint dwError;

            dwError = GetExtendedTcpTable(ptr, ref buffSize, false, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER, 0);

            if (dwError != ERROR_INSUFFICIENT_BUFFER)
                throw new Win32Exception();

            ptr = Marshal.AllocHGlobal(buffSize);

            dwError = GetExtendedTcpTable(ptr, ref buffSize, false, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_LISTENER, 0);

            if (dwError != NO_ERROR)
            {
                Marshal.FreeHGlobal(ptr);
                throw new Win32Exception();
            }

            int count = Marshal.ReadInt32(ptr);
            IntPtr ptrRead = ptr + sizeof(int);
            int sz = Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID));

            var arr = new MIB_TCPROW_OWNER_PID[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(ptrRead, typeof(MIB_TCPROW_OWNER_PID));
                ptrRead += sz;
            }

            Marshal.FreeHGlobal(ptr);

            var conList = arr.Select(p => new Connection
            {
                Local = new IPAddress(p.dwLocalAddr),
                Remote = new IPAddress(p.dwRemoteAddr),
                RemotePort = htons((ushort)p.dwRemotePort),
                LocalPort = htons((ushort)p.dwLocalPort),
                State = p.dwState,
                ProcessId = p.dwOwningPid
            }).ToList();

            return conList;
        }

        public static ushort GetMinecraftPort()
        {
            var javaProcesses = Process.GetProcessesByName("javaw").Select(p => p.Id).ToArray();
            return GetListeners().Find(p => javaProcesses.Contains(p.ProcessId)).LocalPort;
        }
    }

    public class Connection
    {
        public int ProcessId { get; set; }
        public IPAddress Local { get; set; }
        public ushort LocalPort { get; set; }
        public IPAddress Remote { get; set; }
        public ushort RemotePort { get; set; }
        public uint State { get; set; }


        public override string ToString()
        {
            return $"{Local}:{LocalPort} -> {Remote}:{RemotePort} | State: {State} | Process ID: {ProcessId}";
        }
    }
}
