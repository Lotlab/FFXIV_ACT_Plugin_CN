using System;
using System.Collections.Generic;
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
		chart1.Series["Series1"].XValueMember = "Item1";
		chart1.Series["Series1"].YValueMembers = "Item2";
		chart1.Series["Series1"].XValueType = ChartValueType.DateTime;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
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
			string benchmarkType = listView1.SelectedItems[0].SubItems[0].Text;
			BenchmarkStats benchmarkStats2 = _benchmarkRepository.GetBenchmarkStats(benchmarkType);
			chart1.DataSource = benchmarkStats2.History;
			chart1.ChartAreas[0].AxisY.Maximum = benchmarkStats2.MaxTime.TotalMilliseconds;
			chart1.DataBind();
		}
	}

	private void btnCopyToClipboard_Click(object sender, EventArgs e)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ListViewItem item in listView1.Items)
		{
			stringBuilder.AppendLine(string.Join("\t", item.SubItems[0].Text, item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[4].Text, item.SubItems[5].Text));
		}
		if (stringBuilder.Length > 0)
		{
			Clipboard.SetText(stringBuilder.ToString());
		}
	}

	private void btnReset_Click(object sender, EventArgs e)
	{
		_benchmarkRepository.ResetBenchmarkStats(null);
	}

	private void BenchmarkPropertyPage_Resize(object sender, EventArgs e)
	{
		int num = Height - listView1.Location.Y;
		listView1.Height = (num - 10) / 2;
		chart1.Location = new Point
		{
			X = chart1.Location.X,
			Y = listView1.Location.Y + listView1.Height + 5
		};
		chart1.Height = (num - 10) / 2;
		chart1.Width = Width - 50;
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
		this.components = new System.ComponentModel.Container();
		System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
		System.Windows.Forms.DataVisualization.Charting.Legend legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
		System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series();
		this.listView1 = new System.Windows.Forms.ListView();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
		this.btnCopyToClipboard = new System.Windows.Forms.Button();
		this.btnReset = new System.Windows.Forms.Button();
		this.chkWriteToAppData = new System.Windows.Forms.CheckBox();
		this.cmdOpenLog = new System.Windows.Forms.Button();
		this.chkLimitTo10 = new System.Windows.Forms.CheckBox();
		((System.ComponentModel.ISupportInitialize)this.chart1).BeginInit();
		base.SuspendLayout();
		this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[6] { this.columnHeader1, this.columnHeader2, this.columnHeader3, this.columnHeader4, this.columnHeader5, this.columnHeader6 });
		this.listView1.FullRowSelect = true;
		this.listView1.HideSelection = false;
		this.listView1.Location = new System.Drawing.Point(3, 88);
		this.listView1.Name = "listView1";
		this.listView1.Size = new System.Drawing.Size(1379, 399);
		this.listView1.TabIndex = 0;
		this.listView1.UseCompatibleStateImageBehavior = false;
		this.listView1.View = System.Windows.Forms.View.Details;
		this.columnHeader1.Text = "Type";
		this.columnHeader1.Width = 151;
		this.columnHeader2.Text = "Last";
		this.columnHeader2.Width = 100;
		this.columnHeader3.Text = "Average";
		this.columnHeader3.Width = 100;
		this.columnHeader4.Text = "Total";
		this.columnHeader4.Width = 100;
		this.columnHeader5.Text = "Max";
		this.columnHeader5.Width = 100;
		this.columnHeader6.Text = "Count";
		this.columnHeader6.Width = 100;
		this.timer1.Enabled = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(1259, 46);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(100, 28);
		this.checkBox1.TabIndex = 1;
		this.checkBox1.Text = "Enabled";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.Visible = false;
		this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
		chartArea.AxisX.IsLabelAutoFit = false;
		chartArea.AxisX.LabelStyle.Format = "HH:mm:ss";
		chartArea.Name = "ChartArea1";
		this.chart1.ChartAreas.Add(chartArea);
		legend.Enabled = false;
		legend.Name = "Legend1";
		this.chart1.Legends.Add(legend);
		this.chart1.Location = new System.Drawing.Point(3, 493);
		this.chart1.Name = "chart1";
		series.ChartArea = "ChartArea1";
		series.IsVisibleInLegend = false;
		series.Legend = "Legend1";
		series.Name = "Series1";
		series.YValuesPerPoint = 2;
		this.chart1.Series.Add(series);
		this.chart1.Size = new System.Drawing.Size(1379, 471);
		this.chart1.TabIndex = 2;
		this.chart1.Text = "chart1";
		this.btnCopyToClipboard.Location = new System.Drawing.Point(3, 37);
		this.btnCopyToClipboard.Name = "btnCopyToClipboard";
		this.btnCopyToClipboard.Size = new System.Drawing.Size(213, 45);
		this.btnCopyToClipboard.TabIndex = 3;
		this.btnCopyToClipboard.Text = "复制到剪贴板";
		this.btnCopyToClipboard.UseVisualStyleBackColor = true;
		this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
		this.btnReset.Location = new System.Drawing.Point(240, 37);
		this.btnReset.Name = "btnReset";
		this.btnReset.Size = new System.Drawing.Size(213, 45);
		this.btnReset.TabIndex = 4;
		this.btnReset.Text = "重置日志";
		this.btnReset.UseVisualStyleBackColor = true;
		this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
		this.chkWriteToAppData.AutoSize = true;
		this.chkWriteToAppData.Location = new System.Drawing.Point(502, 46);
		this.chkWriteToAppData.Name = "chkWriteToAppData";
		this.chkWriteToAppData.Size = new System.Drawing.Size(158, 28);
		this.chkWriteToAppData.TabIndex = 5;
		this.chkWriteToAppData.Text = "输出到AppData下";
		this.chkWriteToAppData.UseVisualStyleBackColor = true;
		this.cmdOpenLog.Location = new System.Drawing.Point(1040, 37);
		this.cmdOpenLog.Name = "cmdOpenLog";
		this.cmdOpenLog.Size = new System.Drawing.Size(213, 45);
		this.cmdOpenLog.TabIndex = 6;
		this.cmdOpenLog.Text = "打开日志文件";
		this.cmdOpenLog.UseVisualStyleBackColor = true;
		this.cmdOpenLog.Click += new System.EventHandler(this.cmdOpenLog_Click);
		this.chkLimitTo10.AutoSize = true;
		this.chkLimitTo10.Location = new System.Drawing.Point(677, 46);
		this.chkLimitTo10.Name = "chkLimitTo10";
		this.chkLimitTo10.Size = new System.Drawing.Size(148, 28);
		this.chkLimitTo10.TabIndex = 7;
		this.chkLimitTo10.Text = "筛选10ms以上的结果";
		this.chkLimitTo10.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(11f, 24f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.Controls.Add(this.chkLimitTo10);
		base.Controls.Add(this.cmdOpenLog);
		base.Controls.Add(this.chkWriteToAppData);
		base.Controls.Add(this.btnReset);
		base.Controls.Add(this.btnCopyToClipboard);
		base.Controls.Add(this.chart1);
		base.Controls.Add(this.checkBox1);
		base.Controls.Add(this.listView1);
		base.Margin = new System.Windows.Forms.Padding(6);
		base.Name = "BenchmarkPropertyPage";
		base.Size = new System.Drawing.Size(1385, 967);
		base.Resize += new System.EventHandler(this.BenchmarkPropertyPage_Resize);
		((System.ComponentModel.ISupportInitialize)this.chart1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
