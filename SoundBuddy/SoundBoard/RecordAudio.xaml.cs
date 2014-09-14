using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

using Coding4Fun.Toolkit.Audio;
using Coding4Fun.Toolkit.Audio.Helpers;
using Coding4Fun.Toolkit.Controls;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Newtonsoft.Json;

using SoundBoard.Resources;
using SoundBoard.ViewModel;

namespace SoundBoard
{
	public partial class RecordAudio : PhoneApplicationPage
	{
		private MicrophoneRecorder _recorder = new MicrophoneRecorder();
		private string _tempFileName = "tempWav.wav";
		private IsolatedStorageFileStream _audioStream;
	
		public RecordAudio()
		{
			InitializeComponent();

			BuildLocalizedApplicationBar();
		}

		private void BuildLocalizedApplicationBar()
		{
			ApplicationBar = new ApplicationBar();

			ApplicationBarIconButton recordAudioAppBar = new ApplicationBarIconButton();
			recordAudioAppBar.IconUri = new Uri("/Assets/AppBar/save.png", UriKind.Relative);
			recordAudioAppBar.Text = AppResources.AppBarSave;

			recordAudioAppBar.Click += SaveRecordingClick;

			ApplicationBar.Buttons.Add(recordAudioAppBar);
			ApplicationBar.IsVisible = false;
		}

		void SaveRecordingClick(object sender, EventArgs e)
		{
			InputPrompt fileName = new InputPrompt();
			fileName.Title = "Sound Name";
			fileName.Message = "What should we call the sound?";

			fileName.Completed += FileNameCompleted;

			fileName.Show();
		}

		void FileNameCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
		{
			if (e.PopUpResult == PopUpResult.Ok)
			{
				SoundData soundData = new SoundData();
				soundData.FilePath = string.Format("/customAudio/{0}.wav", DateTime.Now.ToFileTime());
				soundData.Title = e.Result;

				using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
				{
					if (!isoStore.DirectoryExists("/customAudio/"))
						isoStore.CreateDirectory("/customAudio/");

					isoStore.MoveFile(_tempFileName, soundData.FilePath);
				
					App.ViewModel.CustomSounds.Items.Add(soundData);
				}

				// serializing takes the collection as well as just the base data
				var data = JsonConvert.SerializeObject(App.ViewModel.CustomSounds);

				// save to settings
				IsolatedStorageSettings.ApplicationSettings[SoundModel.CustomSoundKey] = data;
				IsolatedStorageSettings.ApplicationSettings.Save();

				NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
			}
			
		}

		private void PlayAudioClick(object sender, RoutedEventArgs e)
		{
			_audioPlayer.Play();
		}

		private void RecordAudioChecked(object sender, RoutedEventArgs e)
		{
			PlayAudio.IsEnabled = false;
			ApplicationBar.IsVisible = false;
			RotateCircle.Begin();

			_recorder.Start();
		}

		private void RecordAudioUnchecked(object sender, RoutedEventArgs e)
		{
			_recorder.Stop();

			SaveTempAudio(_recorder.Buffer);

			PlayAudio.IsEnabled = true;
			ApplicationBar.IsVisible = true;
			RotateCircle.Stop();
		}

		private void SaveTempAudio(MemoryStream buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (_audioStream != null)
			{
				_audioPlayer.Stop();
				_audioPlayer.Source = null;
				
				_audioStream.Dispose();
			}

			using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
			{
				if (isoStore.FileExists(_tempFileName))
					isoStore.DeleteFile(_tempFileName);
			
				_tempFileName = string.Format("{0}.wav", DateTime.Now.ToFileTime());

				var bytes = buffer.GetWavAsByteArray(_recorder.SampleRate);

				_audioStream = isoStore.CreateFile(_tempFileName);
				_audioStream.Write(bytes, 0, bytes.Length);

				if (!isoStore.FileExists(_tempFileName))
					return;

				_audioPlayer.SetSource(_audioStream);
			}
		}
	}
}