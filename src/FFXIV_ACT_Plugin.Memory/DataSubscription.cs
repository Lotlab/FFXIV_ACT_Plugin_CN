#define TRACE
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Memory;

public class DataSubscription : IDataSubscription
{
	[CompilerGenerated]
	private NetworkReceivedDelegate m_NetworkReceived;

	[CompilerGenerated]
	private NetworkSentDelegate m_NetworkSent;

	[CompilerGenerated]
	private CombatantAddedDelegate m_CombatantAdded;

	[CompilerGenerated]
	private CombatantRemovedDelegate m_CombatantRemoved;

	[CompilerGenerated]
	private PrimaryPlayerDelegate m_PrimaryPlayerChanged;

	[CompilerGenerated]
	private ZoneChangedDelegate m_ZoneChanged;

	[CompilerGenerated]
	private PlayerStatsChangedDelegate m_PlayerStatsChanged;

	[CompilerGenerated]
	private PartyListChangedDelegate m_PartyListChanged;

	[CompilerGenerated]
	private LogLineDelegate m_LogLine;

	[CompilerGenerated]
	private ParsedLogLineDelegate m_ParsedLogLine;

	[CompilerGenerated]
	private ProcessChangedDelegate m_ProcessChanged;

	public event NetworkReceivedDelegate NetworkReceived
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			NetworkReceivedDelegate val = this.m_NetworkReceived;
			NetworkReceivedDelegate val2;
			do
			{
				val2 = val;
				NetworkReceivedDelegate value2 = (NetworkReceivedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_NetworkReceived, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			NetworkReceivedDelegate val = this.m_NetworkReceived;
			NetworkReceivedDelegate val2;
			do
			{
				val2 = val;
				NetworkReceivedDelegate value2 = (NetworkReceivedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_NetworkReceived, value2, val2);
			}
			while (val != val2);
		}
	}

	public event NetworkSentDelegate NetworkSent
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			NetworkSentDelegate val = this.m_NetworkSent;
			NetworkSentDelegate val2;
			do
			{
				val2 = val;
				NetworkSentDelegate value2 = (NetworkSentDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_NetworkSent, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			NetworkSentDelegate val = this.m_NetworkSent;
			NetworkSentDelegate val2;
			do
			{
				val2 = val;
				NetworkSentDelegate value2 = (NetworkSentDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_NetworkSent, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CombatantAddedDelegate CombatantAdded
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CombatantAddedDelegate val = this.m_CombatantAdded;
			CombatantAddedDelegate val2;
			do
			{
				val2 = val;
				CombatantAddedDelegate value2 = (CombatantAddedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CombatantAdded, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CombatantAddedDelegate val = this.m_CombatantAdded;
			CombatantAddedDelegate val2;
			do
			{
				val2 = val;
				CombatantAddedDelegate value2 = (CombatantAddedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CombatantAdded, value2, val2);
			}
			while (val != val2);
		}
	}

	public event CombatantRemovedDelegate CombatantRemoved
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CombatantRemovedDelegate val = this.m_CombatantRemoved;
			CombatantRemovedDelegate val2;
			do
			{
				val2 = val;
				CombatantRemovedDelegate value2 = (CombatantRemovedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CombatantRemoved, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			CombatantRemovedDelegate val = this.m_CombatantRemoved;
			CombatantRemovedDelegate val2;
			do
			{
				val2 = val;
				CombatantRemovedDelegate value2 = (CombatantRemovedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_CombatantRemoved, value2, val2);
			}
			while (val != val2);
		}
	}

	public event PrimaryPlayerDelegate PrimaryPlayerChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PrimaryPlayerDelegate val = this.m_PrimaryPlayerChanged;
			PrimaryPlayerDelegate val2;
			do
			{
				val2 = val;
				PrimaryPlayerDelegate value2 = (PrimaryPlayerDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PrimaryPlayerChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PrimaryPlayerDelegate val = this.m_PrimaryPlayerChanged;
			PrimaryPlayerDelegate val2;
			do
			{
				val2 = val;
				PrimaryPlayerDelegate value2 = (PrimaryPlayerDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PrimaryPlayerChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event ZoneChangedDelegate ZoneChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ZoneChangedDelegate val = this.m_ZoneChanged;
			ZoneChangedDelegate val2;
			do
			{
				val2 = val;
				ZoneChangedDelegate value2 = (ZoneChangedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoneChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ZoneChangedDelegate val = this.m_ZoneChanged;
			ZoneChangedDelegate val2;
			do
			{
				val2 = val;
				ZoneChangedDelegate value2 = (ZoneChangedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ZoneChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event PlayerStatsChangedDelegate PlayerStatsChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PlayerStatsChangedDelegate val = this.m_PlayerStatsChanged;
			PlayerStatsChangedDelegate val2;
			do
			{
				val2 = val;
				PlayerStatsChangedDelegate value2 = (PlayerStatsChangedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PlayerStatsChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PlayerStatsChangedDelegate val = this.m_PlayerStatsChanged;
			PlayerStatsChangedDelegate val2;
			do
			{
				val2 = val;
				PlayerStatsChangedDelegate value2 = (PlayerStatsChangedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PlayerStatsChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event PartyListChangedDelegate PartyListChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PartyListChangedDelegate val = this.m_PartyListChanged;
			PartyListChangedDelegate val2;
			do
			{
				val2 = val;
				PartyListChangedDelegate value2 = (PartyListChangedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PartyListChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			PartyListChangedDelegate val = this.m_PartyListChanged;
			PartyListChangedDelegate val2;
			do
			{
				val2 = val;
				PartyListChangedDelegate value2 = (PartyListChangedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_PartyListChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public event LogLineDelegate LogLine
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LogLineDelegate val = this.m_LogLine;
			LogLineDelegate val2;
			do
			{
				val2 = val;
				LogLineDelegate value2 = (LogLineDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LogLine, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			LogLineDelegate val = this.m_LogLine;
			LogLineDelegate val2;
			do
			{
				val2 = val;
				LogLineDelegate value2 = (LogLineDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_LogLine, value2, val2);
			}
			while (val != val2);
		}
	}

	public event ParsedLogLineDelegate ParsedLogLine
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ParsedLogLineDelegate val = this.m_ParsedLogLine;
			ParsedLogLineDelegate val2;
			do
			{
				val2 = val;
				ParsedLogLineDelegate value2 = (ParsedLogLineDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ParsedLogLine, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ParsedLogLineDelegate val = this.m_ParsedLogLine;
			ParsedLogLineDelegate val2;
			do
			{
				val2 = val;
				ParsedLogLineDelegate value2 = (ParsedLogLineDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ParsedLogLine, value2, val2);
			}
			while (val != val2);
		}
	}

	public event ProcessChangedDelegate ProcessChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ProcessChangedDelegate val = this.m_ProcessChanged;
			ProcessChangedDelegate val2;
			do
			{
				val2 = val;
				ProcessChangedDelegate value2 = (ProcessChangedDelegate)Delegate.Combine((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ProcessChanged, value2, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected Obj, but got Unknown
			ProcessChangedDelegate val = this.m_ProcessChanged;
			ProcessChangedDelegate val2;
			do
			{
				val2 = val;
				ProcessChangedDelegate value2 = (ProcessChangedDelegate)Delegate.Remove((Delegate?)(object)val2, (Delegate?)(object)value);
				val = Interlocked.CompareExchange(ref this.m_ProcessChanged, value2, val2);
			}
			while (val != val2);
		}
	}

	public void OnNetworkReceived(string connection, long epoch, byte[] message)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_NetworkReceived == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_NetworkReceived).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			NetworkReceivedDelegate val = (NetworkReceivedDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(connection, epoch, message, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnNetworkSent(string connection, long epoch, byte[] message)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_NetworkSent == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_NetworkSent).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			NetworkSentDelegate val = (NetworkSentDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(connection, epoch, message, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnCombatantAdded(object Combatant)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_CombatantAdded == null)
		{
			return;
		}
		Combatant val = ((Combatant)Combatant).Clone();
		Delegate[] invocationList = ((Delegate)(object)this.m_CombatantAdded).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			CombatantAddedDelegate val2 = (CombatantAddedDelegate)invocationList[i];
			if (val2 != null)
			{
				val2.BeginInvoke((object)val, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnCombatantRemoved(object Combatant)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_CombatantRemoved == null)
		{
			return;
		}
		Combatant val = ((Combatant)Combatant).Clone();
		Delegate[] invocationList = ((Delegate)(object)this.m_CombatantRemoved).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			CombatantRemovedDelegate val2 = (CombatantRemovedDelegate)invocationList[i];
			if (val2 != null)
			{
				val2.BeginInvoke((object)val, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnPrimaryPlayerChanged()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_PrimaryPlayerChanged == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_PrimaryPlayerChanged).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			PrimaryPlayerDelegate val = (PrimaryPlayerDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke((AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnZoneChanged(uint ZoneID, string ZoneName)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_ZoneChanged == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_ZoneChanged).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			ZoneChangedDelegate val = (ZoneChangedDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(ZoneID, ZoneName, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnPlayerStatsChanged(object playerStats)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_PlayerStatsChanged == null)
		{
			return;
		}
		Player val = ((Player)playerStats).Clone();
		Delegate[] invocationList = ((Delegate)(object)this.m_PlayerStatsChanged).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			PlayerStatsChangedDelegate val2 = (PlayerStatsChangedDelegate)invocationList[i];
			if (val2 != null)
			{
				val2.BeginInvoke((object)val, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnPartyListChanged(ReadOnlyCollection<uint> partyList, int partySize)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_PartyListChanged == null)
		{
			return;
		}
		ReadOnlyCollection<uint> readOnlyCollection = partyList.ToList().AsReadOnly();
		Delegate[] invocationList = ((Delegate)(object)this.m_PartyListChanged).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			PartyListChangedDelegate val = (PartyListChangedDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(readOnlyCollection, partySize, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnLogLine(uint EventType, uint Seconds, string logline)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_LogLine == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_LogLine).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			LogLineDelegate val = (LogLineDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(EventType, Seconds, logline, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnParsedLogLine(uint sequence, int eventType, string message)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_ParsedLogLine == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_ParsedLogLine).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			ParsedLogLineDelegate val = (ParsedLogLineDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(sequence, eventType, message, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	public void OnProcessChanged(Process process)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (this.m_ProcessChanged == null)
		{
			return;
		}
		Delegate[] invocationList = ((Delegate)(object)this.m_ProcessChanged).GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			ProcessChangedDelegate val = (ProcessChangedDelegate)invocationList[i];
			if (val != null)
			{
				val.BeginInvoke(process, (AsyncCallback)DelegateCallback, (object)null);
			}
		}
	}

	private void DelegateCallback(IAsyncResult ar)
	{
		object asyncDelegate = ((AsyncResult)((ar is AsyncResult) ? ar : null)).AsyncDelegate;
		try
		{
			asyncDelegate.GetType().GetMethod("EndInvoke").Invoke(asyncDelegate, new object[1] { ar });
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Exception in DataSubscription.  Caller Type: [" + asyncDelegate.GetType().FullName + "].  Please pass this error to the plugin authors: " + ex.ToString().Replace(Environment.NewLine, " "));
		}
	}
}
