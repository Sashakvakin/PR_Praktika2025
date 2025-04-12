using ChickenAndPoint.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Postgrest.Exceptions;


namespace ChickenAndPoint
{
    public partial class EditCategoryFoodWindow : Window
    {
        private Категории _categoryToEdit;
        private ImageUrlToBitmapConverter _iconConverter = new ImageUrlToBitmapConverter();

        public EditCategoryFoodWindow(Категории category)
        {
            InitializeComponent();
            _categoryToEdit = category ?? throw new ArgumentNullException(nameof(category));
            Loaded += EditCategoryFoodWindow_Loaded;
        }

        private void EditCategoryFoodWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"Редактирование: {_categoryToEdit.НазваниеКатегории}";
            CategoryNameTextBox.Text = _categoryToEdit.НазваниеКатегории;
            CategoryIconUrlTextBox.Text = _categoryToEdit.СсылкаНаИконку;
            UpdateIconPreview();
            CategoryNameTextBox.Focus();
            CategoryNameTextBox.SelectAll();
        }

        private void UpdateIconPreview()
        {
            string url = CategoryIconUrlTextBox.Text;
            if (!string.IsNullOrWhiteSpace(url))
            {
                object imageSource = _iconConverter.Convert(url, typeof(BitmapImage), null, CultureInfo.CurrentCulture);
                if (imageSource is BitmapImage bmp)
                {
                    IconPreviewImage.Source = bmp;
                    IconPreviewPlaceholder.Visibility = Visibility.Collapsed;
                    IconPreviewImage.Visibility = Visibility.Visible;
                }
                else
                {
                    IconPreviewImage.Source = null;
                    IconPreviewPlaceholder.Visibility = Visibility.Visible;
                    IconPreviewImage.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                IconPreviewImage.Source = null;
                IconPreviewPlaceholder.Visibility = Visibility.Visible;
                IconPreviewImage.Visibility = Visibility.Collapsed;
            }
        }

        private void CategoryIconUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateIconPreview();
        }

        private void SelectIconButton_Click(object sender, RoutedEventArgs e)
        {
            var iconSelector = new CategoryIconSelectorWindow();
            iconSelector.Owner = this;
            bool? result = iconSelector.ShowDialog();

            if (result == true && !string.IsNullOrEmpty(iconSelector.SelectedIconUrl))
            {
                CategoryIconUrlTextBox.Text = iconSelector.SelectedIconUrl;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newCategoryName = CategoryNameTextBox.Text.Trim();
            string newIconUrl = string.IsNullOrWhiteSpace(CategoryIconUrlTextBox.Text) ? null : CategoryIconUrlTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(newCategoryName))
            {
                MessageBox.Show("Название категории не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryNameTextBox.Focus();
                return;
            }

            if (newCategoryName == _categoryToEdit.НазваниеКатегории && newIconUrl == _categoryToEdit.СсылкаНаИконку)
            {
                this.DialogResult = false;
                this.Close();
                return;
            }

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var response = await App.SupabaseClient.From<Категории>()
                    .Where(c => c.Id == _categoryToEdit.Id)
                    .Set(c => c.НазваниеКатегории, newCategoryName)
                    .Set(c => c.СсылкаНаИконку, newIconUrl)
                    .Update();

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Категория '{newCategoryName}' успешно обновлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = await response.ResponseMessage.Content.ReadAsStringAsync();
                    if (errorMsg.Contains("duplicate key value violates unique constraint") && errorMsg.Contains("Категории_название_категории_key"))
                    {
                        MessageBox.Show($"Категория с названием '{newCategoryName}' уже существует.", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Не удалось обновить категорию: {response.ResponseMessage.ReasonPhrase}\n{errorMsg}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обновлении категории: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible)
                {
                    SaveButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}