#define TRACE
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Common.Models;

namespace FFXIV_ACT_Plugin.Memory;

public class DataSubscription : IDataSubscription
{
	public event NetworkReceivedDelegate NetworkReceived;

	public event NetworkSentDelegate NetworkSent;

	public event CombatantAddedDelegate CombatantAdded;

	public event CombatantRemovedDelegate CombatantRemoved;

	public event PrimaryPlayerDelegate PrimaryPlayerChanged;

	public event ZoneChangedDelegate ZoneChanged;

	public event PlayerStatsChangedDelegate PlayerStatsChanged;

	public event PartyListChangedDelegate PartyListChanged;

	public event LogLineDelegate LogLine;

	public event ParsedLogLineDelegate ParsedLogLine;

	public event ProcessChangedDelegate ProcessChanged;

	public void OnNetworkReceived(string connection, long epoch, byte[] message)
	{
		if (NetworkReceived != null)
		{
			Delegate[] invocationList = NetworkReceived.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((NetworkReceivedDelegate)invocationList[i])?.BeginInvoke(connection, epoch, message, DelegateCallback, null);
			}
		}
	}

	public void OnNetworkSent(string connection, long epoch, byte[] message)
	{
		if (NetworkSent != null)
		{
			Delegate[] invocationList = NetworkSent.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((NetworkSentDelegate)invocationList[i])?.BeginInvoke(connection, epoch, message, DelegateCallback, null);
			}
		}
	}

	public void OnCombatantAdded(object Combatant)
	{
		if (CombatantAdded != null)
		{
			Combatant combatant = ((Combatant)Combatant).Clone();
			Delegate[] invocationList = CombatantAdded.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((CombatantAddedDelegate)invocationList[i])?.BeginInvoke(combatant, DelegateCallback, null);
			}
		}
	}

	public void OnCombatantRemoved(object Combatant)
	{
		if (CombatantRemoved != null)
		{
			Combatant combatant = ((Combatant)Combatant).Clone();
			Delegate[] invocationList = CombatantRemoved.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((CombatantRemovedDelegate)invocationList[i])?.BeginInvoke(combatant, DelegateCallback, null);
			}
		}
	}

	public void OnPrimaryPlayerChanged()
	{
		if (PrimaryPlayerChanged != null)
		{
			Delegate[] invocationList = PrimaryPlayerChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((PrimaryPlayerDelegate)invocationList[i])?.BeginInvoke(DelegateCallback, null);
			}
		}
	}

	public void OnZoneChanged(uint ZoneID, string ZoneName)
	{
		if (ZoneChanged != null)
		{
			Delegate[] invocationList = ZoneChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((ZoneChangedDelegate)invocationList[i])?.BeginInvoke(ZoneID, ZoneName, DelegateCallback, null);
			}
		}
	}

	public void OnPlayerStatsChanged(object playerStats)
	{
		if (PlayerStatsChanged != null)
		{
			Player playerStats2 = ((Player)playerStats).Clone();
			Delegate[] invocationList = PlayerStatsChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((PlayerStatsChangedDelegate)invocationList[i])?.BeginInvoke(playerStats2, DelegateCallback, null);
			}
		}
	}

	public void OnPartyListChanged(ReadOnlyCollection<uint> partyList, int partySize)
	{
		if (PartyListChanged != null)
		{
			ReadOnlyCollection<uint> partyList2 = partyList.ToList().AsReadOnly();
			Delegate[] invocationList = PartyListChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((PartyListChangedDelegate)invocationList[i])?.BeginInvoke(partyList2, partySize, DelegateCallback, null);
			}
		}
	}

	public void OnLogLine(uint EventType, uint Seconds, string logline)
	{
		if (LogLine != null)
		{
			Delegate[] invocationList = LogLine.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((LogLineDelegate)invocationList[i])?.BeginInvoke(EventType, Seconds, logline, DelegateCallback, null);
			}
		}
	}

	public void OnParsedLogLine(uint sequence, int eventType, string message)
	{
		if (ParsedLogLine != null)
		{
			Delegate[] invocationList = ParsedLogLine.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((ParsedLogLineDelegate)invocationList[i])?.BeginInvoke(sequence, eventType, message, DelegateCallback, null);
			}
		}
	}

	public void OnProcessChanged(Process process)
	{
		if (ProcessChanged != null)
		{
			Delegate[] invocationList = ProcessChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				((ProcessChangedDelegate)invocationList[i])?.BeginInvoke(process, DelegateCallback, null);
			}
		}
	}

	private void DelegateCallback(IAsyncResult ar)
	{
		object asyncDelegate = (ar as AsyncResult).AsyncDelegate;
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
