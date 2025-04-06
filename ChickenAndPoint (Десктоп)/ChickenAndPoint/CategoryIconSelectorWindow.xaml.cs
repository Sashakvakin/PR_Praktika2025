using Microsoft.Win32;
using MimeMapping;
using Supabase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChickenAndPoint
{
    public partial class CategoryIconSelectorWindow : Window 
    {
        public string SelectedIconUrl { get; private set; } 
        private const string BUCKET_NAME = "category"; 

        public CategoryIconSelectorWindow()
        {
            InitializeComponent();
            Loaded += CategoryIconSelectorWindow_Loaded; 
        }

        private async void CategoryIconSelectorWindow_Loaded(object sender, RoutedEventArgs e) 
        {
            await LoadImagesAsync();
        }

        private async Task LoadImagesAsync()
        {
            LoadingStatus.Visibility = Visibility.Visible;
            ImagesScrollViewer.Visibility = Visibility.Collapsed;
            ImagesItemsControl.ItemsSource = null;

            try
            {
                if (App.SupabaseClient == null)
                {
                    LoadingStatus.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    return;
                }
                var fileList = await App.SupabaseClient.Storage.From(BUCKET_NAME).List();
                if (fileList == null)
                {
                    LoadingStatus.Text = "Не удалось получить список файлов из хранилища иконок.";
                    return;
                }
                if (!fileList.Any())
                {
                    LoadingStatus.Text = "Хранилище иконок пусто.";
                    return;
                }
                var imageUrls = new List<string>();
                var storageRef = App.SupabaseClient.Storage.From(BUCKET_NAME);
                foreach (var file in fileList)
                {
                    if (string.IsNullOrEmpty(file.Name) || file.Id == null) continue;
                    var publicUrl = storageRef.GetPublicUrl(file.Name);
                    if (!string.IsNullOrEmpty(publicUrl))
                    {
                        imageUrls.Add(publicUrl);
                    }
                }
                if (imageUrls.Any())
                {
                    ImagesItemsControl.ItemsSource = imageUrls.OrderBy(url => url).ToList();
                    LoadingStatus.Visibility = Visibility.Collapsed;
                    ImagesScrollViewer.Visibility = Visibility.Visible;
                }
                else
                {
                    LoadingStatus.Text = "Не найдено подходящих иконок в хранилище.";
                }
            }
            catch (Exception ex)
            {
                LoadingStatus.Text = $"Ошибка загрузки иконок: {ex.Message}";
                MessageBox.Show(LoadingStatus.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите иконку для загрузки (оптимально 64x64)",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(filePath);

                SetLoadingState(true, "Загрузка иконки...");
                this.Cursor = Cursors.Wait;

                try
                {
                    if (App.SupabaseClient == null)
                    {
                        MessageBox.Show("Клиент Supabase не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] fileBytes = await Task.Run(() => File.ReadAllBytes(filePath));
                    string contentType = MimeUtility.GetMimeMapping(fileName);

                    var options = new Supabase.Storage.FileOptions
                    {
                        CacheControl = "3600",
                        ContentType = contentType,
                        Upsert = false
                    };

                    var storage = App.SupabaseClient.Storage.From(BUCKET_NAME);
                    var response = await storage.Upload(fileBytes, fileName, options);

                    MessageBox.Show($"Иконка '{fileName}' успешно загружена.", "Загрузка завершена", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadImagesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке иконки: {ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    SetLoadingState(false);
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void SetLoadingState(bool isLoading, string statusText = null)
        {
            if (isLoading)
            {
                LoadingStatus.Text = statusText ?? "Выполнение операции...";
                LoadingStatus.Visibility = Visibility.Visible;
                ImagesScrollViewer.Visibility = Visibility.Collapsed;
                UploadButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
            }
            else
            {
                UploadButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string imageUrl)
            {
                SelectedIconUrl = imageUrl;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}