using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class CustomForm : Form
{
    [DllImport("DwmApi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int attrValue, int attriSize);

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        int enableDarkMode = 1; 
        DwmSetWindowAttribute(Handle, 20, ref enableDarkMode, sizeof(int));
    }
}
