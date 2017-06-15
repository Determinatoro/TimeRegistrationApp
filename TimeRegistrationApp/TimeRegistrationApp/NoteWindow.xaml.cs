using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeRegistrationApp.Webservice;

namespace TimeRegistrationApp
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        private TimeRegistration timeRegistration;
        private MainWindow mainWindow;

        public NoteWindow(MainWindow mainWindow, TimeRegistration timeRegistration)
        {
            InitializeComponent();

            this.timeRegistration = timeRegistration;

            tbNote.Text = timeRegistration.Note;
            tbNote.MaxLength = 50;

            this.mainWindow = mainWindow;

            tbNote.Focus();
            tbNote.SelectAll();
        }

        private void SetNote()
        {
            WebserviceCalls.SetNoteForTimeRegistration(timeRegistration.TimeRegId, tbNote.Text);
            mainWindow.GetTimeRegistrations();
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SetNote();
        }

        private void tbNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetNote();
        }
    }
}
