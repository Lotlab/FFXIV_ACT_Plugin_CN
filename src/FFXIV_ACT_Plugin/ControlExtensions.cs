using System.Windows.Forms;

namespace FFXIV_ACT_Plugin;

public static class ControlExtensions
{
	public static T FindControl<T>(this Control @this, string path) where T : Control
	{
		if (@this == null || path == null)
		{
			return default;
		}
		if (path.Contains("\\"))
		{
			Control val = @this;
			string[] array = path.Split(new char[1] { '\\' });
			foreach (string text in array)
			{
				val = val.Controls[text];
				if (val == null)
				{
					return default;
				}
			}
			return (T)(object)((val is T) ? val : null);
		}
		Control val2 = @this.Controls[path];
		return (T)(object)((val2 is T) ? val2 : null);
	}
}
