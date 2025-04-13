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
using Postgrest.Exceptions;

namespace ChickenAndPoint
{
    public partial class EditFoodWindow : Window
    {
        private DishAdminViewModel _dishViewModel;
        private List<Категории> _availableCategories;
        private ImageUrlToBitmapConverter _imageConverter = new ImageUrlToBitmapConverter();

        public Блюда UpdatedDishData { get; private set; }
        public bool CategoriesMayHaveChanged { get; private set; }
        public bool WasDeleted { get; private set; }

        public EditFoodWindow(DishAdminViewModel dishVm)
        {
            InitializeComponent();
            _dishViewModel = dishVm ?? throw new ArgumentNullException(nameof(dishVm));
            UpdatedDishData = null;
            CategoriesMayHaveChanged = false;
            WasDeleted = false;
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
                    var currentSelectionId = (CategoryComboBox.SelectedItem as Категории)?.Id ?? (Guid?)CategoryComboBox.SelectedValue;
                    CategoryComboBox.ItemsSource = _availableCategories;

                    if (currentSelectionId.HasValue)
                    {
                        var itemToRestore = _availableCategories.FirstOrDefault(c => c.Id == currentSelectionId.Value);
                        if (itemToRestore != null)
                        {
                            CategoryComboBox.SelectedItem = itemToRestore;
                        }
                        else
                        {
                            CategoryComboBox.SelectedIndex = -1;
                        }
                    }
                    else if (_availableCategories.Any())
                    {
                        var initialCategory = _availableCategories.FirstOrDefault(c => c.Id == _dishViewModel.IdКатегории);
                        if (initialCategory != null)
                        {
                            CategoryComboBox.SelectedItem = initialCategory;
                        }
                    }
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

            if (_availableCategories != null)
            {
                var categoryToSelect = _availableCategories.FirstOrDefault(c => c.Id == _dishViewModel.IdКатегории);
                if (categoryToSelect != null)
                {
                    CategoryComboBox.SelectedItem = categoryToSelect;
                }
                else
                {
                    CategoryComboBox.SelectedIndex = -1;
                    if (CategoryComboBox.ItemsSource != null)
                    {
                        bool found = false;
                        foreach (var item in CategoryComboBox.Items)
                        {
                            if (item is Категории cat && cat.Id == _dishViewModel.IdКатегории)
                            {
                                CategoryComboBox.SelectedItem = item;
                                found = true;
                                break;
                            }
                        }
                        if (!found) CategoryComboBox.SelectedIndex = -1;
                    }
                }
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

            Категории selectedCategory = null;
            if (CategoryComboBox.SelectedItem is Категории catItem)
            {
                selectedCategory = catItem;
            }
            else if (CategoryComboBox.SelectedValue is Guid categoryIdFromValue)
            {
                selectedCategory = _availableCategories?.FirstOrDefault(c => c.Id == categoryIdFromValue);
                if (selectedCategory == null)
                {
                    MessageBox.Show("Выбранная категория не найдена. Пожалуйста, выберите категорию из списка.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CategoryComboBox.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите категорию.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                CategoryComboBox.Focus();
                return;
            }

            Guid selectedCategoryId = selectedCategory.Id;

            var dishUpdateForReturn = new Блюда
            {
                Id = _dishViewModel.Id,
                НазваниеБлюда = NameTextBox.Text.Trim(),
                Описание = DescriptionTextBox.Text.Trim(),
                Цена = price,
                СсылкаНаИзображение = string.IsNullOrWhiteSpace(ImageUrlTextBox.Text) ? null : ImageUrlTextBox.Text.Trim(),
                Доступно = AvailableCheckBox.IsChecked ?? false,
                IdКатегории = selectedCategoryId
            };

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            UpdatedDishData = null;
            WasDeleted = false;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var response = await App.SupabaseClient
                    .From<Блюда>()
                    .Where(b => b.Id == _dishViewModel.Id)
                    .Set(b => b.НазваниеБлюда, dishUpdateForReturn.НазваниеБлюда)
                    .Set(b => b.Описание, dishUpdateForReturn.Описание)
                    .Set(b => b.Цена, dishUpdateForReturn.Цена)
                    .Set(b => b.СсылкаНаИзображение, dishUpdateForReturn.СсылкаНаИзображение)
                    .Set(b => b.Доступно, dishUpdateForReturn.Доступно)
                    .Set(b => b.IdКатегории, dishUpdateForReturn.IdКатегории)
                    .Update();

                if (response?.ResponseMessage != null && response.ResponseMessage.IsSuccessStatusCode)
                {
                    UpdatedDishData = dishUpdateForReturn;
                    MessageBox.Show("Данные блюда успешно обновлены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else if (response?.Models == null && response?.ResponseMessage != null && response.ResponseMessage.IsSuccessStatusCode)
                {
                    UpdatedDishData = dishUpdateForReturn;
                    MessageBox.Show("Данные блюда успешно обновлены (модель не возвращена).", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = response?.ResponseMessage != null ? await response.ResponseMessage.Content.ReadAsStringAsync() : "Неизвестная ошибка при обновлении.";
                    string reasonPhrase = response?.ResponseMessage?.ReasonPhrase ?? "N/A";
                    MessageBox.Show($"Не удалось обновить блюдо: {reasonPhrase}\n{errorMsg}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible && this.DialogResult != true)
                {
                    SaveButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatedDishData = null;
            WasDeleted = false;
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmResult = MessageBox.Show(
                $"Вы уверены, что хотите удалить блюдо '{_dishViewModel.НазваниеБлюда}'?\n\nЭто действие необратимо.",
                "Подтверждение удаления блюда",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            WasDeleted = false;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await App.SupabaseClient
                    .From<Блюда>()
                    .Where(b => b.Id == _dishViewModel.Id)
                    .Delete();

                WasDeleted = true;
                MessageBox.Show($"Блюдо '{_dishViewModel.НазваниеБлюда}' успешно удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();

            }
            catch (PostgrestException pgEx)
            {
                if (pgEx.Message.Contains("violates foreign key constraint"))
                {
                    MessageBox.Show($"Невозможно удалить блюдо '{_dishViewModel.НазваниеБлюда}', так как оно используется в других записях (например, в заказах).", "Ошибка удаления (БД)", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Не удалось удалить блюдо (ошибка БД): {pgEx.Message}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении блюда: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible && this.DialogResult != true)
                {
                    SaveButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    DeleteButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }
    }
}