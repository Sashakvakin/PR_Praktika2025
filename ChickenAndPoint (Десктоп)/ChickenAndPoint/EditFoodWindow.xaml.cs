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
using ChickenAndPoint.Admin;

namespace ChickenAndPoint
{
    public partial class EditFoodWindow : Window
    {
        private DishAdminViewModel _dishViewModel;
        private Блюда _originalDishData;
        private List<Категории> _availableCategories;
        private ImageUrlToBitmapConverter _imageConverter = new ImageUrlToBitmapConverter();

        public EditFoodWindow(DishAdminViewModel dishVm)
        {
            InitializeComponent();
            _dishViewModel = dishVm ?? throw new ArgumentNullException(nameof(dishVm));
            _originalDishData = new Блюда { Id = _dishViewModel.Id };
            Loaded += EditFoodWindow_Loaded;
        }

        private async void EditFoodWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            PopulateFields();
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
                    CategoryComboBox.ItemsSource = _availableCategories;
                }
                else
                {
                    _availableCategories = new List<Категории>();
                    MessageBox.Show("Не удалось загрузить список категорий.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                _availableCategories = new List<Категории>();
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateFields()
        {
            NameTextBox.Text = _dishViewModel.НазваниеБлюда;
            DescriptionTextBox.Text = _dishViewModel.Описание;
            PriceTextBox.Text = _dishViewModel.Цена.ToString("F2", CultureInfo.InvariantCulture);
            ImageUrlTextBox.Text = _dishViewModel.СсылкаНаИзображение;
            AvailableCheckBox.IsChecked = _dishViewModel.Доступно;
            CategoryComboBox.SelectedValue = _dishViewModel.IdКатегории;
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
                }
                else
                {
                    PreviewImage.Source = null;
                    PreviewPlaceholder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                PreviewImage.Source = null;
                PreviewPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void ImageUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateImagePreview();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string currentText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
            Regex regex = new Regex(@"^[0-9]*[.,]?[0-9]{0,2}$");
            if (!regex.IsMatch(currentText) && e.Text != "\b")
            {
                e.Handled = true;
            }
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Название блюда не может быть пустым.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Неверный формат цены или цена отрицательная. Используйте точку как разделитель.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return;
            }

            if (CategoryComboBox.SelectedValue == null || !(CategoryComboBox.SelectedValue is Guid))
            {
                MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryComboBox.Focus();
                return;
            }

            Guid selectedCategoryId = (Guid)CategoryComboBox.SelectedValue;

            var dishUpdate = new Блюда
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
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var response = await App.SupabaseClient
                     .From<Блюда>()
                     .Where(b => b.Id == _originalDishData.Id)

                     .Set(b => b.НазваниеБлюда, dishUpdate.НазваниеБлюда)
                     .Set(b => b.Описание, dishUpdate.Описание)
                     .Set(b => b.Цена, dishUpdate.Цена)
                     .Set(b => b.СсылкаНаИзображение, dishUpdate.СсылкаНаИзображение)
                     .Set(b => b.Доступно, dishUpdate.Доступно)
                     .Set(b => b.IdКатегории, dishUpdate.IdКатегории)
                     .Update();

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Данные блюда успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = await response.ResponseMessage.Content.ReadAsStringAsync();
                    MessageBox.Show($"Не удалось обновить блюдо: {response.ResponseMessage.ReasonPhrase}\n{errorMsg}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryFoodWindow();
            addCategoryWindow.Owner = this;

            bool? result = addCategoryWindow.ShowDialog();
            if (result == true && addCategoryWindow.NewCategory != null)
            {
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