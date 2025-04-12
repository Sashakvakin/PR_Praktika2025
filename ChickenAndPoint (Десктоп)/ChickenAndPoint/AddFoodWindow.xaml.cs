using ChickenAndPoint.Admin;
using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MimeMapping;
using Supabase.Storage;
using System.IO;
using System.Windows.Forms;

namespace ChickenAndPoint
{
    public partial class AddFoodWindow : Window
    {
        private List<Категории> _availableCategories;
        private ImageUrlToBitmapConverter _imageConverter = new ImageUrlToBitmapConverter();

        public Блюда NewlyAddedDish { get; private set; }
        public bool CategoriesMayHaveChanged { get; private set; }

        public AddFoodWindow()
        {
            InitializeComponent();
            Loaded += AddFoodWindow_Loaded;
            NewlyAddedDish = null;
            CategoriesMayHaveChanged = false;
        }

        private async void AddFoodWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            AvailableCheckBox.IsChecked = true;
            UpdateImagePreview();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                if (App.SupabaseClient == null) return;
                var response = await App.SupabaseClient.From<Категории>().Select("*").Get();
                if (response?.Models != null)
                {
                    _availableCategories = response.Models.OrderBy(c => c.НазваниеКатегории).ToList();
                    var currentSelectionId = (CategoryComboBox.SelectedItem as Категории)?.Id;
                    CategoryComboBox.ItemsSource = _availableCategories;
                    if (currentSelectionId.HasValue)
                    {
                        var itemToRestore = _availableCategories.FirstOrDefault(c => c.Id == currentSelectionId.Value);
                        if (itemToRestore != null)
                        {
                            CategoryComboBox.SelectedItem = itemToRestore;
                        }
                    }
                }
                else
                {
                    _availableCategories = new List<Категории>();
                    System.Windows.MessageBox.Show("Не удалось загрузить список категорий.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _availableCategories = new List<Категории>();
                System.Windows.MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateImagePreview()
        {
            string url = ImageUrlTextBox.Text;
            if (!string.IsNullOrWhiteSpace(url))
            {
                object imageSource = _imageConverter.Convert(url, typeof(BitmapImage), null, CultureInfo.CurrentCulture);
                if (imageSource is BitmapImage bmp)
                {
                    PreviewImage.Source = bmp;
                    PreviewPlaceholder.Visibility = Visibility.Collapsed;
                    PreviewImage.Visibility = Visibility.Visible;
                }
                else
                {
                    PreviewImage.Source = null;
                    PreviewPlaceholder.Visibility = Visibility.Visible;
                    PreviewImage.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                PreviewImage.Source = null;
                PreviewPlaceholder.Visibility = Visibility.Collapsed;
                PreviewImage.Visibility = Visibility.Collapsed;
            }
        }

        private void ImageUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateImagePreview();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            string currentText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
            Regex regex = new Regex(@"^[0-9]*[.,]?[0-9]{0,2}$");
            if (!regex.IsMatch(currentText) && e.Text != "\b")
            {
                e.Handled = true;
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            var imageSelectorWindow = new FoodImageWindow();
            imageSelectorWindow.Owner = this;
            bool? result = imageSelectorWindow.ShowDialog();
            if (result == true && !string.IsNullOrEmpty(imageSelectorWindow.SelectedImageUrl))
            {
                ImageUrlTextBox.Text = imageSelectorWindow.SelectedImageUrl;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                System.Windows.MessageBox.Show("Название блюда не может быть пустым.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }
            if (!decimal.TryParse(PriceTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                System.Windows.MessageBox.Show("Неверный формат цены или цена отрицательная. Используйте точку как разделитель.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }
            if (!(CategoryComboBox.SelectedItem is Категории selectedCategory))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryComboBox.Focus();
                return;
            }
            Guid selectedCategoryId = selectedCategory.Id;

            var newDish = new Блюда
            {
                НазваниеБлюда = NameTextBox.Text.Trim(),
                Описание = DescriptionTextBox.Text.Trim(),
                Цена = price,
                СсылкаНаИзображение = string.IsNullOrWhiteSpace(ImageUrlTextBox.Text) ? null : ImageUrlTextBox.Text.Trim(),
                Доступно = AvailableCheckBox.IsChecked ?? false,
                IdКатегории = selectedCategoryId
            };

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            NewlyAddedDish = null;

            try
            {
                if (App.SupabaseClient == null)
                {
                    System.Windows.MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var response = await App.SupabaseClient
                    .From<Блюда>()
                    .Insert(newDish);

                if (response?.Models != null && response.Models.Any())
                {
                    NewlyAddedDish = response.Models.First();
                    System.Windows.MessageBox.Show("Новое блюдо успешно добавлено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = response?.ResponseMessage != null ? await response.ResponseMessage.Content.ReadAsStringAsync() : "Неизвестная ошибка при вставке.";
                    System.Windows.MessageBox.Show($"Не удалось добавить блюдо: {errorMsg}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Произошла ошибка при добавлении: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible && this.DialogResult != true)
                {
                    SaveButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NewlyAddedDish = null;
            this.DialogResult = false;
            this.Close();
        }

        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryFoodWindow();
            addCategoryWindow.Owner = this;

            bool? result = addCategoryWindow.ShowDialog();
            if (result == true && addCategoryWindow.NewCategory != null)
            {
                CategoriesMayHaveChanged = true;
                await LoadCategoriesAsync();
                var newlyAddedCategory = _availableCategories?
                    .FirstOrDefault(c => c.Id == addCategoryWindow.NewCategory.Id);

                if (newlyAddedCategory != null)
                {
                    CategoryComboBox.SelectedItem = newlyAddedCategory;
                }
            }
        }
    }
}