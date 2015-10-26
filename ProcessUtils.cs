using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MineSweeperReader
{
	public class ProcessUtils
	{
	    // ReSharper disable InconsistentNaming
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        // ReSharper restore InconsistentNaming

        #region P/Invoke
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        #endregion

        /// <summary>	
		/// Process from which to read		
		/// </summary>
		public Process ReadProcess { get; set; }

		private IntPtr _process = IntPtr.Zero;
        public IntPtr Process { get { return _process; } }

		public void OpenProcess(bool read = true, bool write = true)
		{
		    var flags = read ? PROCESS_VM_READ : 0;
		    if (write)
		        flags |= PROCESS_VM_OPERATION | PROCESS_VM_WRITE;
			_process = OpenProcess(flags, 1, (uint)ReadProcess.Id);
		}

		public void CloseHandle()
		{
		    var iRetValue = CloseHandle(_process);
		    if (iRetValue == 0)
				throw new Exception("CloseHandle failed");
		}

	    public byte[] ReadProcessMemory(int address, uint bytesToRead, out int bytesRead)
	    {
	        return ReadProcessMemory((IntPtr)address, bytesToRead, out bytesRead);
	    }

	    public byte[] ReadProcessMemory(IntPtr memoryAddress, uint bytesToRead, out int bytesRead)
		{
			var buffer = new byte[bytesToRead];
			
			IntPtr ptrBytesReaded;
			ReadProcessMemory(_process, memoryAddress, buffer, bytesToRead, out ptrBytesReaded);
			
			bytesRead = ptrBytesReaded.ToInt32();
			return buffer;
		}

	    public int WriteProcessMemory(int address, byte[] data)
	    {
	        return WriteProcessMemory((IntPtr) address, data);
	    }

	    public int WriteProcessMemory(IntPtr memoryAddress, byte[] data)
	    {
            IntPtr res;
	        return !WriteProcessMemory(_process, memoryAddress, data, data.Length, out res) ? 0 : res.ToInt32();
	    }

	    public int GetWindowPositionX()
	    {
	        var position = new Rect();
            if (!GetWindowRect(ReadProcess.MainWindowHandle, ref position))
	            return 0;
	        return position.Left;
	    }

        public int GetWindowPositionY()
        {
            var position = new Rect();
            if (!GetWindowRect(ReadProcess.MainWindowHandle, ref position))
                return 0;
            return position.Top;
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
          int dwExtraInfo);


        public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        public const uint MOUSEEVENTF_LEFTUP = 0x04;
        public const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

	    public void BringToFront()
	    {
            IntPtr handle = ReadProcess.MainWindowHandle;
            if (IsIconic(handle))
                ShowWindow(handle, 9); // SW_RESTORE
            SetForegroundWindow(handle);
	    }
	}
}
