using System.ComponentModel;

namespace SoundBoard.ViewModel
{
	public class SoundData : INotifyPropertyChanged
	{
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				if (value == _title) 
					return;

				_title = value;
				OnPropertyChanged("Title");
			}
		}
		private string _title;

		public string FilePath
		{
			get
			{
				return _filePath;
			}
			set
			{
				if (value == _filePath)
					return;

				_filePath = value;
				OnPropertyChanged("FilePath");
			}
		}
		private string _filePath;

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;

			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
