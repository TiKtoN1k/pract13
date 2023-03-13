using Microsoft.Win32;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace pract13
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<FileInfo> playlist;
        int repeat = 0;
        int randomized = 0;
        private void polzunok()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    SngSlider.Value = media.Position.Ticks;
                    currentTimer.Text = media.Position.Duration().ToString();
                    if (media.NaturalDuration.HasTimeSpan)
                    {
                        maxTimer.Text = (media.NaturalDuration.TimeSpan - media.Position.Duration()).ToString();
                        if (maxTimer.Text == new TimeSpan(0).ToString())
                        {
                            if (repeat == 0)
                            {
                                if (MusicList.SelectedIndex == MusicList.Items.Count - 1)
                                    MusicList.SelectedIndex = 0;
                                else
                                    MusicList.SelectedIndex = MusicList.SelectedIndex + 1;
                            }
                            else
                                media.Position = new TimeSpan(0);
                        }
                    }
                });
                Thread.Sleep(100);
            }
        }
        private void zagruzkaPlaylista(List<FileInfo> files)
        {
            MusicList.Items.Clear();
            for (int i = 0; i < files.Count(); i++)
            {
                MusicList.Items.Add(files[i].Name);
            }
            if (MusicList.Items.Count != 0) MusicList.SelectedIndex = 0;
        }
        private void playMusic(string file)
        {
            media.Source = new Uri(file);
            media.Play();
            media.Volume = 0.1;
        }
        public MainWindow()
        {
            InitializeComponent();
            Thread thread = new Thread(_ =>
            {
                polzunok();
            });
            thread.Start();

        }
        MediaPlayer mediaPlayer = new MediaPlayer();
        private void Vibor_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog() { IsFolderPicker = true };
            CommonFileDialogResult opened = fileDialog.ShowDialog();
            if (opened == CommonFileDialogResult.Ok)
            {
                string folder = fileDialog.FileName;
                string[] files = Directory.GetFiles(folder);
                playlist = new List<FileInfo>();
                for (int i = 0; i < files.GetLength(0); i++)
                {
                    FileInfo fileInfo = new FileInfo(files[i]);
                    if (fileInfo.Extension != ".mp3") continue;
                    playlist.Add(fileInfo);
                }
                playlist = playlist.OrderBy(x => x.Name).ToList();
                zagruzkaPlaylista(playlist);
            }

        }
        private void MusicList_changed(object sender, EventArgs e)
        {
            if (MusicList.SelectedIndex != -1)
            {
                FileInfo selectedMusic = playlist[MusicList.SelectedIndex];
                playMusic(selectedMusic.FullName);
            }

        }
        private void Proshlaya_Click(object sender, RoutedEventArgs e)
        {
            if (MusicList.SelectedIndex == 0)
                MusicList.SelectedIndex = MusicList.Items.Count - 1;
            else
                MusicList.SelectedIndex = MusicList.SelectedIndex - 1;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            media.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }

        private void Sledushaya_Click(object sender, RoutedEventArgs e)
        {
            if (MusicList.SelectedIndex == MusicList.Items.Count - 1)
                MusicList.SelectedIndex = 0;
            else
                MusicList.SelectedIndex = MusicList.SelectedIndex + 1;
        }

        private void Povtor_Click(object sender, RoutedEventArgs e)
        {
            if (repeat == 0) repeat = 1;
            else repeat = 0;
        }

        private void SngSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(SngSlider.Value));
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            SngSlider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }
        private void Peremeshka_Click(object sender, RoutedEventArgs e)
        {

            if (randomized == 0) randomized = 1;
            else randomized = 0;

            if (randomized == 1)
            {
                int n = playlist.Count;
                Random rng = new Random();
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    FileInfo value = playlist[k];
                    playlist[k] = playlist[n];
                    playlist[n] = value;
                }
            }
            else
            {
                playlist = playlist.OrderBy(x => x.Name).ToList();
            }
            zagruzkaPlaylista(playlist);
        }
    }
}
