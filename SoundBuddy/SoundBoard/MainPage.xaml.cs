﻿using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using SoundBoard.Resources;
using SoundBoard.ViewModel;

namespace SoundBoard
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();

			DataContext = App.ViewModel;
			
			BuildLocalizedApplicationBar();
		}

		// Load data for the ViewModel Items
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (!App.ViewModel.IsDataLoaded)
			{
				App.ViewModel.LoadData();
			}
		}

		private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LongListSelector selector = sender as LongListSelector;

			// verifying our sender is actually a LongListSelector
			if (selector == null)
				return;

			SoundData data = selector.SelectedItem as SoundData;

			// verifying our sender is actually SoundData
			if (data == null)
				return;

			// is file a custom recorded file?
			if (File.Exists(data.FilePath))
			{
				// by default, MediaElement will auto play.  No need to stop / start
				_audioPlayer.Source = new Uri(data.FilePath, UriKind.RelativeOrAbsolute);
			}
			else
			{
				using (var storageFolder = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var stream = new IsolatedStorageFileStream(data.FilePath, FileMode.Open, storageFolder))
					{
						_audioPlayer.SetSource(stream);
					}
				}
			}

			// resetting selected so we can play the same sound over and over again
			selector.SelectedItem = null;
		}

		private void BuildLocalizedApplicationBar()
		{
		    ApplicationBar = new ApplicationBar();

			ApplicationBarIconButton recordAudioAppBar = new ApplicationBarIconButton();
			recordAudioAppBar.IconUri = new Uri("/Assets/AppBar/microphone.png", UriKind.Relative);
			recordAudioAppBar.Text = AppResources.AppBarRecord;
			
			recordAudioAppBar.Click += RecordAudioClick;
		    
		    ApplicationBarMenuItem aboutAppBar = new ApplicationBarMenuItem();
			aboutAppBar.Text = AppResources.AppBarAbout;

			aboutAppBar.Click += AboutClick;

			ApplicationBar.Buttons.Add(recordAudioAppBar);
			ApplicationBar.MenuItems.Add(aboutAppBar);
		}

		void AboutClick(object sender, EventArgs e)
		{
			AboutPrompt aboutMe = new AboutPrompt();
			
			aboutMe.Show("Soham Pal", "@sohampal", "aelfric@outlook.com", "http://www.facebook.com/aelfric.anon");
		}

		private void RecordAudioClick(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/RecordAudio.xaml", UriKind.RelativeOrAbsolute));
		}
	}
}