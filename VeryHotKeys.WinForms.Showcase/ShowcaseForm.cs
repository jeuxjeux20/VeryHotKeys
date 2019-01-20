using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeryHotKeys.WinForms.Showcase
{
    public partial class ShowcaseForm : GlobalHotKeyForm
    {
        public ShowcaseForm()
        {
            InitializeComponent();
            AddHotKeyRegisterer(FirstDemoMessageBox, HotKeyMods.Control | HotKeyMods.Shift, ConsoleKey.O);
            AddHotKeyRegisterer(() => new ShowcaseForm().Show(), HotKeyMods.Shift, ConsoleKey.O);
            AddHotKeyRegisterer(Close, HotKeyMods.Control | HotKeyMods.Shift | HotKeyMods.Alt, ConsoleKey.O);
            AddHotKeyRegisterer(BigSurprise, HotKeyMods.Alt, ConsoleKey.W);
        }

        private readonly Random _random = new Random();
        private void BigSurprise(object sender, EventArgs e)
        {
            GetFocus();
            MessageBox.Show("Catch me if you can!", "Surprise 2!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Task.Run(async () =>
            {
                Action d = () => Left += _random.Next(-20, 25);
                while (true)
                {
                    this.BeginInvoke(d);
                    await Task.Delay(45);
                }
            });
        }

        private void FirstDemoMessageBox(object sender, EventArgs e)
        {
            GetFocus();
            MessageBox.Show("Wow a surprise!", "Surprise wowie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetFocus()
        {
            TopMost = true;
            TopMost = false;
            Activate();
            Focus();
        }
    }
}
