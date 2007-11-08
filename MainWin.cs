using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public unsafe partial class MainWin : Form
{
    const int WM_SIZING         = 0x0214;
    const int WMSZ_LEFT         = 1;
    const int WMSZ_RIGHT        = 2;
    const int WMSZ_TOP          = 3;
    const int WMSZ_TOPLEFT      = 4;
    const int WMSZ_TOPRIGHT     = 5;
    const int WMSZ_BOTTOM       = 6;
    const int WMSZ_BOTTOMLEFT   = 7;
    const int WMSZ_BOTTOMRIGHT  = 8;

    bool[,] cellStates;
    bool mouseMove;
    Point mouseMovePrev;

    public MainWin()
    {
        InitializeComponent();

        // Initialize the state;
        this.BackgroundImage = new Bitmap(1, 1);
        cellStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { 514, 386 }, new[] { -1, -1 });
        Render();
    }

    private void Step_Tick(object sender, EventArgs e)
    {
        // Create the new state array.
        var newStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { 514, 386 }, new[] { -1, -1 });
        for (int y = 0; y < 384; y++)
            for (int x = 0; x < 512; x++)
            {
                // Count the neighboring live cells.
                var liveCount = 0;
                for (int j = y - 1; j <= y + 1; j++)
                    for (int i = x - 1; i <= x + 1; i++)
                        if (cellStates[i, j]) liveCount++;

                // Evaluate the cell live state.
                if (cellStates[x, y])
                    newStates[x, y] = liveCount >= 3 && liveCount <= 4;
                else
                    newStates[x, y] = liveCount == 3;
            }

        // Replace the state and render it.
        cellStates = newStates;
        Render();
    }

    void Render()
    {
        Bitmap bmp = new Bitmap(1024, 768);
        BitmapData dat = bmp.LockBits(new Rectangle(0, 0, 1024, 768), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        for (int y = 0; y < 768; y++)
        {
            uint* pt = (uint*)((byte*)dat.Scan0 + dat.Stride * y);
            for (int x = 0; x < 1024; x++, pt++)
                *pt = cellStates[x >> 1, y >> 1] ? 0xffffffff : 0xff000000;
        }
        bmp.UnlockBits(dat);

        Image old = this.BackgroundImage;
        old.Dispose();
        this.BackgroundImage = bmp;
    }

    private void MainWin_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == 'r')
        {
            Random r = new Random();
            for (int y = 0; y < 384; y++)
                for (int x = 0; x < 512; x++)
                    cellStates[x, y] = r.NextDouble() < .2;
            Render();
        }
    }

    private void MainWin_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
            // When clicked by right, start or stop the time.
            this.Step.Enabled = !this.Step.Enabled;
    }

    private void MainWin_MouseDown(object sender, MouseEventArgs e)
    {
        if ((int)(e.Button & MouseButtons.Left) != 0)
        {
            mouseMove = true;
            cellStates[e.X >> 1, e.Y >> 1] = !cellStates[e.X >> 1, e.Y >> 1];
            Render();

            mouseMovePrev = new Point(e.X, e.Y);
        }
    }

    void MainWin_MouseMove(object sender, MouseEventArgs e)
    {
        if (mouseMove)
        {
            var dist = Math.Sqrt(Math.Pow(e.X - mouseMovePrev.X, 2) + Math.Pow(e.Y - mouseMovePrev.Y, 2)) / 2;
            var dx = (e.X - mouseMovePrev.X) / dist;
            var dy = (e.Y - mouseMovePrev.Y) / dist;
            double x = mouseMovePrev.X, y = mouseMovePrev.Y;
            for (int i = 0; i < dist; i++, x += dx, y += dy)
            {
                int xx = (int)x, yy = (int)y;
                cellStates[xx >> 1, yy >> 1] = !cellStates[xx >> 1, yy >> 1];
            }
            mouseMovePrev = new Point(e.X, e.Y);
        }
    }

    void MainWin_MouseUp(object sender, MouseEventArgs e)
    {
        if ((int)(e.Button & MouseButtons.Left) != 0)
        {
            mouseMove = false;
            Render();
        }
    }

    private void MainWin_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.C)
        {
            cellStates = (bool[,])Array.CreateInstance(typeof(bool), new[] { 514, 386 }, new[] { -1, -1 });
            Render();
        }
        else if (e.KeyData == Keys.Escape)
            this.Close();
    }
}
