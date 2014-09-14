using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SoundBoard.ViewModel
{
	public class SoundGroup : INotifyPropertyChanged
	{
		public SoundGroup()
		{
			Items = new ObservableCollection<SoundData>();
		}

		public ObservableCollection<SoundData> Items { get; private set; }

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
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;

			if (handler != null) 
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
