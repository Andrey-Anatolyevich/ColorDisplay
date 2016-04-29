using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ColorTester
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Color DefaultColor = Colors.White;

		public delegate void OnClipboardChangeEventHandler(ClipboardFormat format, object data);
		public static event OnClipboardChangeEventHandler OnClipboardChange;

		public MainWindow()
		{
			InitializeComponent();

			ClipboardMonitor.Start();
			ClipboardMonitor.OnClipboardChange += this.ClipboardChanged;
		}

		private void ClipboardChanged(ClipboardFormat format, object data)
		{
			if (format != ClipboardFormat.Text)
				return;

			try
			{
				string input = data as string;
				if (string.IsNullOrEmpty(input))
				{
					this.SetBrush(this.DefaultColor);
					return;
				}

				List<string> parts = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				List<byte> result = new List<byte>();

				// приведём все значения к числовому виду
				for (int partNumber = 0; partNumber < parts.Count; partNumber++)
				{
					string current = parts[partNumber].Trim();

					// числовое значение
					if (current.All(x => char.IsNumber(x)))
					{
						int medVal = int.Parse(current);
						result.Add(Convert.ToByte(medVal));
					}

					// hex
					else if (current.StartsWith("0x")
						&& current.Length == 4)
					{
						current = current.Replace("0x", "");
						int value = int.Parse(current, NumberStyles.HexNumber);
						result.Add(Convert.ToByte(value));
					}

					// not supported
					else
						throw new Exception("Bad code");
				}

				Color inputColor = this.DefaultColor;
				int numberOfParts = result.Count();
				if (numberOfParts == 3)
					inputColor = Color.FromRgb(result[0], result[1], result[2]);
				else if (numberOfParts == 4)
					inputColor = Color.FromArgb(result[0], result[1], result[2], result[3]);

				this.SetBrush(inputColor);
			}
			catch
			{
				this.SetBrush(this.DefaultColor);
			}
		}

		private void SetBrush(Color colorToUse)
		{
			this.Dispatcher.BeginInvoke((Action)(() =>
			{
				Brush newBrush = new SolidColorBrush(colorToUse);
				this.gridMain.Background = newBrush;
			}));
		}
	}
}
