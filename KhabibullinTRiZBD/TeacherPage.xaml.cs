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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KhabibullinTRiZBD
{
    /// <summary>
    /// Логика взаимодействия для TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        public int CountRecords, CountPage, CurrentPage = 0;
        public TeacherPage()
        {
            InitializeComponent();


            var currentPrepod = InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.ToList();

            AgentsListView.ItemsSource = currentPrepod;

            SortComboBox.SelectedIndex = 0;
            FiltrComboBox.SelectedIndex = 0;

            UpdateTeachers();
        }
        public void UpdateTeachers()
        {
            var currentPrepod = InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.ToList();

            if (SortComboBox.SelectedIndex == 0)
            {
                currentPrepod = currentPrepod.OrderBy(p => p.Kod_prepod).ToList();
            }
            if (SortComboBox.SelectedIndex == 1)
            {
                currentPrepod = currentPrepod.OrderBy(p => p.GetFIO).ToList();
            }
            if (SortComboBox.SelectedIndex == 2)
            {
                currentPrepod = currentPrepod.OrderByDescending(p => p.GetFIO).ToList();
            }
            if (SortComboBox.SelectedIndex == 3)
            {
                currentPrepod = currentPrepod.OrderBy(p => p.Stazh).ToList();
            }
            if (SortComboBox.SelectedIndex == 4)
            {
                currentPrepod = currentPrepod.OrderByDescending(p => p.Stazh).ToList();
            }
            if (SortComboBox.SelectedIndex == 5)
            {
                currentPrepod = currentPrepod.OrderBy(p => p.Date_rozhd).ToList();
            }
            if (SortComboBox.SelectedIndex == 6)
            {
                currentPrepod = currentPrepod.OrderByDescending(p => p.Date_rozhd).ToList();
            }

            if (FiltrComboBox.SelectedIndex == 0)
            {
                currentPrepod = currentPrepod.Where(p => (p.Dolzhnost.ToLower().Contains("ассистент") || p.Dolzhnost.ToLower().Contains("старший преподаватель") || p.Dolzhnost.ToLower().Contains("профессор") || p.Dolzhnost.ToLower().Contains("доцент") || p.Dolzhnost.ToLower().Contains("мужской") || p.Dolzhnost.ToLower().Contains("женский"))).ToList();
            }
            if (FiltrComboBox.SelectedIndex == 1)
            {
                currentPrepod = currentPrepod.Where(p => p.Dolzhnost.ToLower().Contains("ассистент")).ToList();
            }
            if (FiltrComboBox.SelectedIndex == 2)
            {
                currentPrepod = currentPrepod.Where(p => (p.Dolzhnost.ToLower().Contains("старший преподаватель"))).ToList();
            }
            if (FiltrComboBox.SelectedIndex == 3)
            {
                currentPrepod = currentPrepod.Where(p => (p.Dolzhnost.ToLower().Contains("профессор"))).ToList();
            }
            if (FiltrComboBox.SelectedIndex == 4)
            {
                currentPrepod = currentPrepod.Where(p => (p.Dolzhnost.ToLower().Contains("доцент"))).ToList();
            }

            currentPrepod = currentPrepod.Where(p => p.Lastname.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Firstname.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Patronymic.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            AgentsListView.ItemsSource = currentPrepod;

            TBlockAllCount.Text = Convert.ToString(InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.ToList().Count());
            TBlockCount.Text = Convert.ToString(currentPrepod.Count());
        }


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTeachers();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTeachers();
        }

        private void FiltrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTeachers();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Prepodavatel));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void AgentsListView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                InformationayaSystemaVUZAEntities.getInstance().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentsListView.ItemsSource = InformationayaSystemaVUZAEntities.getInstance().Prepodavatel.ToList();
                UpdateTeachers();
            }
        }
    }
}
