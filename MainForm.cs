using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MineSweeperReader
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
        private Label lblWidth;
        private Label lblHeight;
        private Label lblMines;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private TextBox txtMines;
        private Button btnRead;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

        private System.Windows.Forms.PictureBox[,] ButtonArray = null;
        private Button button2;
		private System.Windows.Forms.Control[] MainControls = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblMines = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.txtMines = new System.Windows.Forms.TextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(8, 16);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 0;
            this.lblWidth.Text = "Width:";
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(112, 16);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 1;
            this.lblHeight.Text = "Height:";
            // 
            // lblMines
            // 
            this.lblMines.AutoSize = true;
            this.lblMines.Location = new System.Drawing.Point(216, 16);
            this.lblMines.Name = "lblMines";
            this.lblMines.Size = new System.Drawing.Size(38, 13);
            this.lblMines.TabIndex = 2;
            this.lblMines.Text = "Mines:";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(48, 12);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.ReadOnly = true;
            this.txtWidth.Size = new System.Drawing.Size(48, 20);
            this.txtWidth.TabIndex = 3;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(152, 12);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.ReadOnly = true;
            this.txtHeight.Size = new System.Drawing.Size(48, 20);
            this.txtHeight.TabIndex = 4;
            // 
            // txtMines
            // 
            this.txtMines.Location = new System.Drawing.Point(256, 12);
            this.txtMines.Name = "txtMines";
            this.txtMines.ReadOnly = true;
            this.txtMines.Size = new System.Drawing.Size(48, 20);
            this.txtMines.TabIndex = 5;
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(8, 40);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(296, 24);
            this.btnRead.TabIndex = 6;
            this.btnRead.Text = "Read MineSweeper Memory";
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(8, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(293, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Input";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(655, 317);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.txtMines);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.lblMines);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Name = "Form1";
            this.Opacity = 0.7D;
            this.Text = "MineSweeper Memory Reader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

	    private void btnRead_Click(object sender, System.EventArgs e)
	    {
	        Read(false);
	    }

        private void Read(bool injectBack)
        {
	        var oldSize = 0;
            if (txtWidth.Text.Length != 0)
		        oldSize = int.Parse(txtWidth.Text) * int.Parse(txtHeight.Text);

		    const int iWidthAddress = 0x1005334;
            const int iHeightAddress = 0x1005338;
            const int iMinesAddress = 0x1005330;
            const int iCellBaseAddress = 0x1005340;

			var pReader = new ProcessUtils();
			var myProcesses = Process.GetProcessesByName(@"winmine");
			
			// take first instance of minesweeper you find
			if (myProcesses.Length == 0)
			{
				MessageBox.Show(@"No MineSweeper process found!");
				return;
			}
			pReader.ReadProcess = myProcesses[0];

			// open process in read memory mode
			pReader.OpenProcess();
			
			int bytesRead;

            var boardWidth = pReader.ReadProcessMemory((IntPtr)iWidthAddress, 1, out bytesRead)[0];
			txtWidth.Text = boardWidth.ToString();
		
            var boardHeight = pReader.ReadProcessMemory((IntPtr)iHeightAddress, 1, out bytesRead)[0];
			txtHeight.Text = boardHeight.ToString();

            var iMines = pReader.ReadProcessMemory((IntPtr)iMinesAddress, 1, out bytesRead)[0];
			txtMines.Text = iMines.ToString();

			for (var i = oldSize - 1; i >= 0; --i)
                Controls.RemoveAt(Controls.Count - 1);
			ButtonArray = new PictureBox[boardWidth, boardHeight];
            var data = new byte[boardWidth, boardHeight];
		    for (var y = 0; y < boardHeight; y++)
		    {
		        for (var x = 0; x < boardWidth; x++)
		        {
                    ButtonArray[x, y] = new PictureBox
		            {
		                Location = new Point(20 + x * 16, 100 + y * 16),
		                Name = "",
		                Size = new Size(16, 16),
		            };

                    var iCellAddress = iCellBaseAddress + (32 * (y + 1)) + (x + 1);
		            data[x, y] = pReader.ReadProcessMemory(iCellAddress, 1, out bytesRead)[0];

		            Controls.Add(ButtonArray[x, y]);
		        }
		    }

		    var remainingMineCount = iMines;
		    for (var x = 0; x < boardWidth; ++x)
		    {
		        for (var y = 0; y < boardHeight; ++y)
		        {
                    // If this is not a mine nor a flagged mine
		            if (data[x, y] != 0x8F && data[x, y] != 0x8E)
		            {
                        var nearbyCount = 0x40;
		                // 1 2 3
		                // 4   5
		                // 6 7 8
		                if (x > 0 && y > 0)              nearbyCount += (data[x - 1, y - 1] & 0x80) == 0x80 ? 1 : 0;
		                if (y > 0)                       nearbyCount += (data[x, y - 1] & 0x80) == 0x80 ? 1 : 0;
		                if (x + 1 < boardWidth && y > 0) nearbyCount += (data[x + 1, y - 1] & 0x80) == 0x80 ? 1 : 0;

		                if (x > 0)              nearbyCount += (data[x - 1, y] & 0x80) == 0x80 ? 1 : 0;
		                if (x + 1 < boardWidth) nearbyCount += (data[x + 1, y] & 0x80) == 0x80 ? 1 : 0;

		                if (x > 0 && y + 1 < boardHeight)              nearbyCount += (data[x - 1, y + 1] & 0x80) == 0x80 ? 1 : 0;
		                if (y + 1 < boardHeight)                       nearbyCount += (data[x, y + 1] & 0x80) == 0x80 ? 1 : 0;
		                if (x + 1 < boardWidth && y + 1 < boardHeight) nearbyCount += (data[x + 1, y + 1] & 0x80) == 0x80 ? 1 : 0;
		                data[x, y] = (byte)nearbyCount;
		            }
		            else if (data[x, y] == 0x8F)
		            {
		                remainingMineCount -= 1;
                        if (remainingMineCount > 0)
		                    data[x, y] = 0x8E; // Mark as flagged mine
		            }

		            var sourceImage = Properties.Resources.Sprites;
                    var destImage = new Bitmap(sourceImage.Height / 16, sourceImage.Width);
                    var imageIndex = 1;
		            switch (data[x, y])
		            {
		                case 0x40: imageIndex = 15; break;
                        case 0x8E: case 0x8F: imageIndex = 5; break;
                        default:
		                    if (data[x, y] > 0x40 & data[x, y] <= 0x48)
		                        imageIndex = 14 - (data[x, y] - 0x40) + 1;
		                    break;
		            }
                    using (var gfx = Graphics.FromImage(destImage))
                    {
                        gfx.DrawImage(sourceImage, new RectangleF(0, 0, 16, 16),
                            // ReSharper disable once PossibleLossOfFraction
                            new RectangleF(0, imageIndex * sourceImage.Height / 16, 16, 16),
                            GraphicsUnit.Pixel);
                        ButtonArray[x, y].Image = destImage;
                    }
		        }
		    }

            if (!injectBack)
                pReader.CloseHandle();
            else
                new Thread(() =>
                {
                    // Give focus just in case
                    pReader.BringToFront();

                    for (var x = 0; x < boardWidth; ++x) 
                    {
                        for (var y = 0; y < boardHeight; ++y)
                        {
                            if ((data[x, y] & 0x80) == 0x80)
                                continue;
                            // Calculate position
                            var clickX = pReader.GetWindowPositionX() + 28 + 16 * x;
                            var clickY = pReader.GetWindowPositionY() + 114 + 16 * y;

                            ClickOnPoint(pReader.Process, new Point(clickX, clickY));
                        }
                    }

                    // close process handle
                    pReader.CloseHandle();
                }).Start();
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			MainControls = new Control[this.Controls.Count];
			this.Controls.CopyTo(MainControls,0);
		}

        private void button2_Click(object sender, EventArgs e)
        {
            Read(true);
        }

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

#pragma warning disable 649
        internal struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

#pragma warning restore 649

        public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
        {
            var oldPos = Cursor.Position;

            // get screen coordinates
            ClientToScreen(wndHandle, ref clientPoint);

            // set cursor on coords, and press mouse
            Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

            var inputMouseDown = new INPUT {Type = 0};
            // input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; // left button down

            var inputMouseUp = new INPUT {Type = 0};
            // input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; // left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            // return mouse 
            Cursor.Position = oldPos;
        }
	}
}
