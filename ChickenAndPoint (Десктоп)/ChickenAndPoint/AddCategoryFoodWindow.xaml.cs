using ChickenAndPoint.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace ChickenAndPoint
{
    public partial class AddCategoryFoodWindow : Window
    {
        public Категории NewCategory { get; private set; }
        private ImageUrlToBitmapConverter _iconConverter = new ImageUrlToBitmapConverter();

        public AddCategoryFoodWindow()
        {
            InitializeComponent();
            CategoryNameTextBox.Focus();
            UpdateIconPreview(); 
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


        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = CategoryNameTextBox.Text.Trim();
            string iconUrl = string.IsNullOrWhiteSpace(CategoryIconUrlTextBox.Text) ? null : CategoryIconUrlTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Название категории не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryNameTextBox.Focus();
                return;
            }

            var newCategoryData = new Категории
            {
                НазваниеКатегории = categoryName,
                СсылкаНаИконку = iconUrl
            };

            AddButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var response = await App.SupabaseClient.From<Категории>().Insert(newCategoryData);

                if (response?.Models != null && response.Models.Any())
                {
                    NewCategory = response.Models.First();
                    MessageBox.Show($"Категория '{NewCategory.НазваниеКатегории}' успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = response?.ResponseMessage != null ? await response.ResponseMessage.Content.ReadAsStringAsync() : "Неизвестная ошибка при вставке категории.";
                    if (errorMsg.Contains("duplicate key value violates unique constraint"))
                    {
                        MessageBox.Show($"Категория с названием '{categoryName}' уже существует.", "Ошибка добавления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Не удалось добавить категорию: {errorMsg}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении категории: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible)
                {
                    AddButton.IsEnabled = true;
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