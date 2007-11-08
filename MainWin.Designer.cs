using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

partial class MainWin
{
    IContainer components = null;
    private Timer Step;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    void InitializeComponent()
    {
        this.components = new Container();
        this.Step = new Timer(this.components);
        this.SuspendLayout();

        this.Step.Interval = 1;
        this.Step.Tick += new EventHandler(this.Step_Tick);

        this.ControlBox = this.MaximizeBox = this.MinimizeBox = false;
        this.DoubleBuffered = true;
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Game of Life";
        this.WindowState = FormWindowState.Maximized;
        this.MouseUp += new MouseEventHandler(this.MainWin_MouseUp);
        this.MouseClick += new MouseEventHandler(this.MainWin_MouseClick);
        this.MouseDown += new MouseEventHandler(this.MainWin_MouseDown);
        this.KeyPress += new KeyPressEventHandler(this.MainWin_KeyPress);
        this.MouseMove += new MouseEventHandler(this.MainWin_MouseMove);
        this.KeyDown += new KeyEventHandler(this.MainWin_KeyDown);
        this.ResumeLayout(false);
    }
}
