using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;

namespace FFXIV_ACT_Plugin;

public class BenchmarkPropertyPage : UserControl
{
	private readonly IBenchmarkRepository _benchmarkRepository;

	private readonly ISettingsMediator _settingsMediator;

	private readonly Dictionary<string, DateTime> _lastTimestamps = new Dictionary<string, DateTime>();

	private IContainer components;

	private ListView listView1;

	private Timer timer1;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private ColumnHeader columnHeader3;

	private ColumnHeader columnHeader4;

	private ColumnHeader columnHeader5;

	private CheckBox checkBox1;

	private Chart chart1;

	private ColumnHeader columnHeader6;

	private Button btnCopyToClipboard;

	private Button btnReset;

	private CheckBox chkWriteToAppData;

	private Button cmdOpenLog;

	private CheckBox chkLimitTo10;

	public BenchmarkPropertyPage(IBenchmarkRepository benchmarkRepository, ISettingsMediator settingsMediator)
	{
		_benchmarkRepository = benchmarkRepository;
		_settingsMediator = settingsMediator;
		InitializeComponent();
		((ChartNamedElementCollection<Series>)(object)chart1.Series)["Series1"].XValueMember = "Item1";
		((ChartNamedElementCollection<Series>)(object)chart1.Series)["Series1"].YValueMembers = "Item2";
		((ChartNamedElementCollection<Series>)(object)chart1.Series)["Series1"].XValueType = (ChartValueType)8;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected Obj, but got Unknown
		if (checkBox1.Checked != _settingsMediator.ParseSettings.EnableBenchmarks)
		{
			checkBox1.Checked = _settingsMediator.ParseSettings.EnableBenchmarks;
		}
		if (!checkBox1.Checked)
		{
			return;
		}
		List<Tuple<DateTime, string>> list = new List<Tuple<DateTime, string>>();
		foreach (BenchmarkStats benchmarkStats in from x in _benchmarkRepository.GetAllBenchmarkStats()
			orderby x.BenchmarkType
			select x)
		{
			if (!listView1.Items.ContainsKey(benchmarkStats.BenchmarkType))
			{
				listView1.Items.Add(new ListViewItem(new string[6]
				{
					benchmarkStats.BenchmarkType,
					benchmarkStats.LastMeasuredTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture),
					benchmarkStats.AverageTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture),
					benchmarkStats.TotalTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture),
					benchmarkStats.MaxTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture),
					benchmarkStats.Count.ToString(CultureInfo.InvariantCulture)
				})).Name = benchmarkStats.BenchmarkType;
			}
			else
			{
				listView1.Items[benchmarkStats.BenchmarkType].SubItems[1].Text = benchmarkStats.LastMeasuredTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture);
				listView1.Items[benchmarkStats.BenchmarkType].SubItems[2].Text = benchmarkStats.AverageTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture);
				listView1.Items[benchmarkStats.BenchmarkType].SubItems[3].Text = benchmarkStats.TotalTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture);
				listView1.Items[benchmarkStats.BenchmarkType].SubItems[4].Text = benchmarkStats.MaxTime.TotalMilliseconds.ToString("0.000", CultureInfo.InvariantCulture);
				listView1.Items[benchmarkStats.BenchmarkType].SubItems[5].Text = benchmarkStats.Count.ToString(CultureInfo.InvariantCulture);
			}
			if (chkWriteToAppData.Checked)
			{
				if (_lastTimestamps.ContainsKey(benchmarkStats.BenchmarkType))
				{
					list.AddRange(from x in benchmarkStats.History
						where x != null && x.Item2 > (chkLimitTo10.Checked ? 10.0 : 0.0) && x.Item1 > _lastTimestamps[benchmarkStats.BenchmarkType]
						select new Tuple<DateTime, string>(x.Item1, $"{x.Item1:HH:mm:ss.fff}\t{x.Item2:0.000}\t{benchmarkStats.BenchmarkType}"));
				}
				else
				{
					list.AddRange(from x in benchmarkStats.History
						where x != null && x.Item2 > (chkLimitTo10.Checked ? 10.0 : 0.0)
						select new Tuple<DateTime, string>(x.Item1, $"{x.Item1:HH:mm:ss.fff}\t{x.Item2:0.000}\t{benchmarkStats.BenchmarkType}"));
				}
			}
			if (benchmarkStats.History.Any((Tuple<DateTime, double> x) => x != null))
			{
				_lastTimestamps[benchmarkStats.BenchmarkType] = benchmarkStats.History.Where((Tuple<DateTime, double> x) => x != null).Max((Tuple<DateTime, double> x) => x.Item1);
			}
		}
		if (chkWriteToAppData.Checked)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Tuple<DateTime, string> item in list.OrderBy((Tuple<DateTime, string> x) => x.Item1))
			{
				stringBuilder.AppendLine(item.Item2);
			}
			File.AppendAllText($"{ACTWrapper.AppDataFolder.FullName}\\ACT_FFXIV_Plugin_Benchmark_{DateTime.Now:yyyy-MM-dd}.txt", stringBuilder.ToString());
		}
		if (listView1.SelectedItems.Count > 0)
		{
			string text = listView1.SelectedItems[0].SubItems[0].Text;
			BenchmarkStats benchmarkStats2 = _benchmarkRepository.GetBenchmarkStats(text);
			chart1.DataSource = benchmarkStats2.History;
			((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Maximum = benchmarkStats2.MaxTime.TotalMilliseconds;
			chart1.DataBind();
		}
	}

	private void btnCopyToClipboard_Click(object sender, EventArgs e)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ListViewItem item in listView1.Items)
		{
			ListViewItem val = item;
			stringBuilder.AppendLine(string.Join("\t", new string[6]
			{
				val.SubItems[0].Text,
				val.SubItems[1].Text,
				val.SubItems[2].Text,
				val.SubItems[3].Text,
				val.SubItems[4].Text,
				val.SubItems[5].Text
			}));
		}
		if (stringBuilder.Length > 0)
		{
			Clipboard.SetText(stringBuilder.ToString());
		}
	}

	private void btnReset_Click(object sender, EventArgs e)
	{
		_benchmarkRepository.ResetBenchmarkStats((string)null);
	}

	private void BenchmarkPropertyPage_Resize(object sender, EventArgs e)
	{
		int num = ((Control)this).Height - ((Control)listView1).Location.Y;
		((Control)listView1).Height = (num - 10) / 2;
		((Control)chart1).Location = new Point
		{
			X = ((Control)chart1).Location.X,
			Y = ((Control)listView1).Location.Y + ((Control)listView1).Height + 5
		};
		((Control)chart1).Height = (num - 10) / 2;
		((Control)chart1).Width = ((Control)this).Width - 50;
	}

	private void cmdOpenLog_Click(object sender, EventArgs e)
	{
		string text = (from x in Directory.GetFiles(ACTWrapper.AppDataFolder.FullName ?? "")
			where x.Contains("ACT_FFXIV_Plugin_Benchmark_") && x.EndsWith(".txt", ignoreCase: true, CultureInfo.InvariantCulture)
			orderby x descending
			select x).FirstOrDefault();
		if (text != null)
		{
			Process.Start(text);
		}
	}

	private void InitializeComponent()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected Obj, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected Obj, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected Obj, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected Obj, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected Obj, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected Obj, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected Obj, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected Obj, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected Obj, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected Obj, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected Obj, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected Obj, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected Obj, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected Obj, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected Obj, but got Unknown
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		components = new Container();
		ChartArea val = new ChartArea();
		Legend val2 = new Legend();
		Series val3 = new Series();
		listView1 = new ListView();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		columnHeader3 = new ColumnHeader();
		columnHeader4 = new ColumnHeader();
		columnHeader5 = new ColumnHeader();
		columnHeader6 = new ColumnHeader();
		timer1 = new Timer(components);
		checkBox1 = new CheckBox();
		chart1 = new Chart();
		btnCopyToClipboard = new Button();
		btnReset = new Button();
		chkWriteToAppData = new CheckBox();
		cmdOpenLog = new Button();
		chkLimitTo10 = new CheckBox();
		((ISupportInitialize)chart1).BeginInit();
		((Control)this).SuspendLayout();
		listView1.Columns.AddRange(new ColumnHeader[6] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6 });
		listView1.FullRowSelect = true;
		listView1.HideSelection = false;
		((Control)listView1).Location = new Point(3, 88);
		((Control)listView1).Name = "listView1";
		((Control)listView1).Size = new Size(1379, 399);
		((Control)listView1).TabIndex = 0;
		listView1.UseCompatibleStateImageBehavior = false;
		listView1.View = (View)1;
		columnHeader1.Text = "Type";
		columnHeader1.Width = 151;
		columnHeader2.Text = "Last";
		columnHeader2.Width = 100;
		columnHeader3.Text = "Average";
		columnHeader3.Width = 100;
		columnHeader4.Text = "Total";
		columnHeader4.Width = 100;
		columnHeader5.Text = "Max";
		columnHeader5.Width = 100;
		columnHeader6.Text = "Count";
		columnHeader6.Width = 100;
		timer1.Enabled = true;
		timer1.Interval = 1000;
		timer1.Tick += timer1_Tick;
		((Control)checkBox1).AutoSize = true;
		((Control)checkBox1).Location = new Point(1259, 46);
		((Control)checkBox1).Name = "checkBox1";
		((Control)checkBox1).Size = new Size(100, 28);
		((Control)checkBox1).TabIndex = 1;
		((Control)checkBox1).Text = "Enabled";
		((ButtonBase)checkBox1).UseVisualStyleBackColor = true;
		((Control)checkBox1).Visible = false;
		chart1.BorderlineDashStyle = (ChartDashStyle)5;
		val.AxisX.IsLabelAutoFit = false;
		val.AxisX.LabelStyle.Format = "HH:mm:ss";
		((ChartNamedElement)val).Name = "ChartArea1";
		((Collection<ChartArea>)(object)chart1.ChartAreas).Add(val);
		val2.Enabled = false;
		((ChartNamedElement)val2).Name = "Legend1";
		((Collection<Legend>)(object)chart1.Legends).Add(val2);
		((Control)chart1).Location = new Point(3, 493);
		((Control)chart1).Name = "chart1";
		val3.ChartArea = "ChartArea1";
		((DataPointCustomProperties)val3).IsVisibleInLegend = false;
		val3.Legend = "Legend1";
		((ChartNamedElement)val3).Name = "Series1";
		val3.YValuesPerPoint = 2;
		((Collection<Series>)(object)chart1.Series).Add(val3);
		chart1.Size = new Size(1379, 471);
		((Control)chart1).TabIndex = 2;
		((Control)chart1).Text = "chart1";
		((Control)btnCopyToClipboard).Location = new Point(3, 37);
		((Control)btnCopyToClipboard).Name = "btnCopyToClipboard";
		((Control)btnCopyToClipboard).Size = new Size(213, 45);
		((Control)btnCopyToClipboard).TabIndex = 3;
		((Control)btnCopyToClipboard).Text = "复制到剪贴板";
		((ButtonBase)btnCopyToClipboard).UseVisualStyleBackColor = true;
		((Control)btnCopyToClipboard).Click += btnCopyToClipboard_Click;
		((Control)btnReset).Location = new Point(240, 37);
		((Control)btnReset).Name = "btnReset";
		((Control)btnReset).Size = new Size(213, 45);
		((Control)btnReset).TabIndex = 4;
		((Control)btnReset).Text = "重置日志";
		((ButtonBase)btnReset).UseVisualStyleBackColor = true;
		((Control)btnReset).Click += btnReset_Click;
		((Control)chkWriteToAppData).AutoSize = true;
		((Control)chkWriteToAppData).Location = new Point(502, 46);
		((Control)chkWriteToAppData).Name = "chkWriteToAppData";
		((Control)chkWriteToAppData).Size = new Size(158, 28);
		((Control)chkWriteToAppData).TabIndex = 5;
		((Control)chkWriteToAppData).Text = "输出到AppData下";
		((ButtonBase)chkWriteToAppData).UseVisualStyleBackColor = true;
		((Control)cmdOpenLog).Location = new Point(1040, 37);
		((Control)cmdOpenLog).Name = "cmdOpenLog";
		((Control)cmdOpenLog).Size = new Size(213, 45);
		((Control)cmdOpenLog).TabIndex = 6;
		((Control)cmdOpenLog).Text = "打开日志文件";
		((ButtonBase)cmdOpenLog).UseVisualStyleBackColor = true;
		((Control)cmdOpenLog).Click += cmdOpenLog_Click;
		((Control)chkLimitTo10).AutoSize = true;
		((Control)chkLimitTo10).Location = new Point(677, 46);
		((Control)chkLimitTo10).Name = "chkLimitTo10";
		((Control)chkLimitTo10).Size = new Size(148, 28);
		((Control)chkLimitTo10).TabIndex = 7;
		((Control)chkLimitTo10).Text = "筛选10ms以上的结果";
		((ButtonBase)chkLimitTo10).UseVisualStyleBackColor = true;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(11f, 24f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Control)this).AutoSize = true;
		((UserControl)this).AutoSizeMode = (AutoSizeMode)0;
		((Control)this).Controls.Add((Control)(object)chkLimitTo10);
		((Control)this).Controls.Add((Control)(object)cmdOpenLog);
		((Control)this).Controls.Add((Control)(object)chkWriteToAppData);
		((Control)this).Controls.Add((Control)(object)btnReset);
		((Control)this).Controls.Add((Control)(object)btnCopyToClipboard);
		((Control)this).Controls.Add((Control)(object)chart1);
		((Control)this).Controls.Add((Control)(object)checkBox1);
		((Control)this).Controls.Add((Control)(object)listView1);
		((Control)this).Margin = new Padding(6);
		((Control)this).Name = "BenchmarkPropertyPage";
		((Control)this).Size = new Size(1385, 967);
		((Control)this).Resize += BenchmarkPropertyPage_Resize;
		((ISupportInitialize)chart1).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
