using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KhabibullinTRiZBD
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private TeacherPage _teacherPage = new TeacherPage();
        private Prepodavatel _currentPrepods = new Prepodavatel();
        private int _StashTemp;
        DateTime? PrepodBirthday;
        public AddEditPage(Prepodavatel SelectedPrepod)
        {
            InitializeComponent();

            IDTB.Visibility = Visibility.Visible;
            
            PickupCombo.ItemsSource = InformationayaSystemaVUZAEntities.getInstance().Kafedra.Select(p => p.Nazvanie_kaf).ToList();
             
            TBStazh.Visibility = Visibility.Visible;
            IDTB.IsEnabled = false;
            TBStazh.IsEnabled = false;

            if (SelectedPrepod != null)
            {
                _currentPrepods = SelectedPrepod;
                PickupCombo.SelectedIndex = _currentPrepods.Kod_kafedry - 1;
            }
            
            DataContext = _currentPrepods;

            PrepodBirthday = _currentPrepods.Date_rozhd;
            dtPicker.Text = PrepodBirthday.ToString();

            if (SelectedPrepod == null)
            {
                _currentPrepods.Dolzhnost = (ComboType.Items[0] as ComboBoxItem)?.Content.ToString() ?? "";
                PickupCombo.SelectedIndex = 0;
                _currentPrepods.Photo = "\\Teachers\\picture.png";
                _currentPrepods.God_post = DateTime.Now.Year;
                _currentPrepods.Kod_kafedry = InformationayaSystemaVUZAEntities.getInstance().Kafedra.Count();
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.InitialDirectory = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Teachers");

            if (myOpenFileDialog.ShowDialog() == true)
            {
                string fullPath = myOpenFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(fullPath);
                string relativePath = System.IO.Path.Combine("\\Teachers", fileName);
                
                _currentPrepods.Photo = relativePath;

                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentPrepods.Firstname))
                errors.AppendLine("Укажите имя преподавателя");
            if (string.IsNullOrWhiteSpace(_currentPrepods.Lastname))
                errors.AppendLine("Укажите фамилию преподавателя");
            if (string.IsNullOrWhiteSpace(_currentPrepods.Patronymic))
                errors.AppendLine("Укажите отчество преподавателя");

            if (string.IsNullOrWhiteSpace(_currentPrepods.God_post.ToString()))
                errors.AppendLine("Укажите год поступления преподавателя");
            else if (_currentPrepods.God_post > DateTime.Now.Year || _currentPrepods.God_post <= DateTime.Now.Year - 80)
                errors.AppendLine("Год поступления преподавателя указан неверно");

            if (PrepodBirthday == null)
                errors.AppendLine("Укажите дату рождения преподавателя");
            else
            {
                if (PrepodBirthday > DateTime.Today)
                    errors.AppendLine("Дата рождения преподавателя указана неверно");
            }


            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите должность преподавателя");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            
            _currentPrepods.Date_rozhd = PrepodBirthday;
            _currentPrepods.Stazh = Convert.ToInt32(TBStazh.Text);
            _currentPrepods.Kod_kafedry = PickupCombo.SelectedIndex + 1;

            MessageBox.Show($"Date_rozhd {_currentPrepods.Date_rozhd}");
            if (_currentPrepods.Kod_prepod == 0)
            {

                int allCount = InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.Count();
                _currentPrepods.Kod_prepod = allCount + 1;

                InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.Add(_currentPrepods);
            }

            try
            {
                InformationayaSystemaVUZAEntities.getInstance().SaveChanges();

                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentPrepod = (sender as Button).DataContext as Prepodavatel;

            var currentKafedry = InformationayaSystemaVUZAEntities.getInstance().Kafedra.ToList();

            currentKafedry = currentKafedry.Where(p => (p.Lastname == currentPrepod.Lastname && p.Firstname == currentPrepod.Firstname && p.Patronymic == currentPrepod.Patronymic)).ToList();

            if (currentKafedry.Count != 0)
            {
                MessageBox.Show($"Невозможно выполнить удаление, так как этот преподаватель является заведующим факультета");
            }
            else
            {
                if (MessageBox.Show($"Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.Remove(currentPrepod);
                        InformationayaSystemaVUZAEntities.getInstance().SaveChanges();

                        _teacherPage.AgentsListView.ItemsSource = InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.ToList();
                        Manager.MainFrame.Navigate(new TeacherPage());
                        _teacherPage.UpdateTeachers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TBGodPost.Text))
            {
                int year;
                if (int.TryParse(TBGodPost.Text, out year))
                {
                    TBStazh.Text = $"{DateTime.Now.Year - year}";
                }
            }
            else
            {
                TBStazh.Text = "0";
            }
        }
        private void dtPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            PrepodBirthday = (DateTime)(((DatePicker)sender).SelectedDate);
        }
    }
}
