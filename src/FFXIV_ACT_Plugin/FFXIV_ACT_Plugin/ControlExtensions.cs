using System.Windows.Forms;

namespace FFXIV_ACT_Plugin;

public static class ControlExtensions
{
	public static T FindControl<T>(this Control @this, string path) where T : Control
	{
		if (@this == null || path == null)
		{
			return null;
		}
		if (path.Contains("\\"))
		{
			Control control = @this;
			string[] array = path.Split('\\');
			foreach (string key in array)
			{
				control = control.Controls[key];
				if (control == null)
				{
					return null;
				}
			}
			return control as T;
		}
		return @this.Controls[path] as T;
	}
}
