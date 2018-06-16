﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RimWorld
{
	// Token: 0x02000690 RID: 1680
	public sealed class HaulDestinationManager
	{
		// Token: 0x0600238E RID: 9102 RVA: 0x00131664 File Offset: 0x0012FA64
		public HaulDestinationManager(Map map)
		{
			this.map = map;
			this.groupGrid = new SlotGroup[map.Size.x, map.Size.y, map.Size.z];
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x0600238F RID: 9103 RVA: 0x001316CC File Offset: 0x0012FACC
		public IEnumerable<IHaulDestination> AllHaulDestinations
		{
			get
			{
				return this.allHaulDestinationsInOrder;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06002390 RID: 9104 RVA: 0x001316E8 File Offset: 0x0012FAE8
		public List<IHaulDestination> AllHaulDestinationsListForReading
		{
			get
			{
				return this.allHaulDestinationsInOrder;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06002391 RID: 9105 RVA: 0x00131704 File Offset: 0x0012FB04
		public List<IHaulDestination> AllHaulDestinationsListInPriorityOrder
		{
			get
			{
				return this.allHaulDestinationsInOrder;
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06002392 RID: 9106 RVA: 0x00131720 File Offset: 0x0012FB20
		public IEnumerable<SlotGroup> AllGroups
		{
			get
			{
				return this.allGroupsInOrder;
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06002393 RID: 9107 RVA: 0x0013173C File Offset: 0x0012FB3C
		public List<SlotGroup> AllGroupsListForReading
		{
			get
			{
				return this.allGroupsInOrder;
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06002394 RID: 9108 RVA: 0x00131758 File Offset: 0x0012FB58
		public List<SlotGroup> AllGroupsListInPriorityOrder
		{
			get
			{
				return this.allGroupsInOrder;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06002395 RID: 9109 RVA: 0x00131774 File Offset: 0x0012FB74
		public IEnumerable<IntVec3> AllSlots
		{
			get
			{
				for (int i = 0; i < this.allGroupsInOrder.Count; i++)
				{
					List<IntVec3> cellsList = this.allGroupsInOrder[i].CellsList;
					int j = 0;
					while (j < this.allGroupsInOrder.Count)
					{
						yield return cellsList[j];
						i++;
					}
				}
				yield break;
			}
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x001317A0 File Offset: 0x0012FBA0
		public void AddHaulDestination(IHaulDestination haulDestination)
		{
			if (this.allHaulDestinationsInOrder.Contains(haulDestination))
			{
				Log.Error("Double-added haul destination " + haulDestination.ToStringSafe<IHaulDestination>(), false);
			}
			else
			{
				this.allHaulDestinationsInOrder.Add(haulDestination);
				IList<IHaulDestination> list = this.allHaulDestinationsInOrder;
				if (HaulDestinationManager.<>f__mg$cache0 == null)
				{
					HaulDestinationManager.<>f__mg$cache0 = new Comparison<IHaulDestination>(HaulDestinationManager.CompareHaulDestinationPrioritiesDescending);
				}
				list.InsertionSort(HaulDestinationManager.<>f__mg$cache0);
				ISlotGroupParent slotGroupParent = haulDestination as ISlotGroupParent;
				if (slotGroupParent != null)
				{
					SlotGroup slotGroup = slotGroupParent.GetSlotGroup();
					if (slotGroup == null)
					{
						Log.Error("ISlotGroupParent gave null slot group: " + slotGroupParent.ToStringSafe<ISlotGroupParent>(), false);
					}
					else
					{
						this.allGroupsInOrder.Add(slotGroup);
						IList<SlotGroup> list2 = this.allGroupsInOrder;
						if (HaulDestinationManager.<>f__mg$cache1 == null)
						{
							HaulDestinationManager.<>f__mg$cache1 = new Comparison<SlotGroup>(HaulDestinationManager.CompareSlotGroupPrioritiesDescending);
						}
						list2.InsertionSort(HaulDestinationManager.<>f__mg$cache1);
						List<IntVec3> cellsList = slotGroup.CellsList;
						for (int i = 0; i < cellsList.Count; i++)
						{
							this.SetCellFor(cellsList[i], slotGroup);
						}
						this.map.listerHaulables.Notify_SlotGroupChanged(slotGroup);
						this.map.listerMergeables.Notify_SlotGroupChanged(slotGroup);
					}
				}
			}
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x001318CC File Offset: 0x0012FCCC
		public void RemoveHaulDestination(IHaulDestination haulDestination)
		{
			if (!this.allHaulDestinationsInOrder.Contains(haulDestination))
			{
				Log.Error("Removing haul destination that isn't registered " + haulDestination.ToStringSafe<IHaulDestination>(), false);
			}
			else
			{
				this.allHaulDestinationsInOrder.Remove(haulDestination);
				ISlotGroupParent slotGroupParent = haulDestination as ISlotGroupParent;
				if (slotGroupParent != null)
				{
					SlotGroup slotGroup = slotGroupParent.GetSlotGroup();
					if (slotGroup == null)
					{
						Log.Error("ISlotGroupParent gave null slot group: " + slotGroupParent.ToStringSafe<ISlotGroupParent>(), false);
					}
					else
					{
						this.allGroupsInOrder.Remove(slotGroup);
						List<IntVec3> cellsList = slotGroup.CellsList;
						for (int i = 0; i < cellsList.Count; i++)
						{
							IntVec3 intVec = cellsList[i];
							this.groupGrid[intVec.x, intVec.y, intVec.z] = null;
						}
						this.map.listerHaulables.Notify_SlotGroupChanged(slotGroup);
						this.map.listerMergeables.Notify_SlotGroupChanged(slotGroup);
					}
				}
			}
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x001319C8 File Offset: 0x0012FDC8
		public void Notify_HaulDestinationChangedPriority()
		{
			IList<IHaulDestination> list = this.allHaulDestinationsInOrder;
			if (HaulDestinationManager.<>f__mg$cache2 == null)
			{
				HaulDestinationManager.<>f__mg$cache2 = new Comparison<IHaulDestination>(HaulDestinationManager.CompareHaulDestinationPrioritiesDescending);
			}
			list.InsertionSort(HaulDestinationManager.<>f__mg$cache2);
			IList<SlotGroup> list2 = this.allGroupsInOrder;
			if (HaulDestinationManager.<>f__mg$cache3 == null)
			{
				HaulDestinationManager.<>f__mg$cache3 = new Comparison<SlotGroup>(HaulDestinationManager.CompareSlotGroupPrioritiesDescending);
			}
			list2.InsertionSort(HaulDestinationManager.<>f__mg$cache3);
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x00131A28 File Offset: 0x0012FE28
		private static int CompareHaulDestinationPrioritiesDescending(IHaulDestination a, IHaulDestination b)
		{
			return ((int)b.GetStoreSettings().Priority).CompareTo((int)a.GetStoreSettings().Priority);
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x00131A5C File Offset: 0x0012FE5C
		private static int CompareSlotGroupPrioritiesDescending(SlotGroup a, SlotGroup b)
		{
			return ((int)b.Settings.Priority).CompareTo((int)a.Settings.Priority);
		}

		// Token: 0x0600239B RID: 9115 RVA: 0x00131A90 File Offset: 0x0012FE90
		public SlotGroup SlotGroupAt(IntVec3 loc)
		{
			return this.groupGrid[loc.x, loc.y, loc.z];
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x00131AC8 File Offset: 0x0012FEC8
		public ISlotGroupParent SlotGroupParentAt(IntVec3 loc)
		{
			SlotGroup slotGroup = this.SlotGroupAt(loc);
			return (slotGroup == null) ? null : slotGroup.parent;
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x00131AF8 File Offset: 0x0012FEF8
		public void SetCellFor(IntVec3 c, SlotGroup group)
		{
			if (this.SlotGroupAt(c) != null)
			{
				Log.Error(string.Concat(new object[]
				{
					group,
					" overwriting slot group square ",
					c,
					" of ",
					this.SlotGroupAt(c)
				}), false);
			}
			this.groupGrid[c.x, c.y, c.z] = group;
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x00131B6C File Offset: 0x0012FF6C
		public void ClearCellFor(IntVec3 c, SlotGroup group)
		{
			if (this.SlotGroupAt(c) != group)
			{
				Log.Error(string.Concat(new object[]
				{
					group,
					" clearing group grid square ",
					c,
					" containing ",
					this.SlotGroupAt(c)
				}), false);
			}
			this.groupGrid[c.x, c.y, c.z] = null;
		}

		// Token: 0x040013E5 RID: 5093
		private Map map;

		// Token: 0x040013E6 RID: 5094
		private List<IHaulDestination> allHaulDestinationsInOrder = new List<IHaulDestination>();

		// Token: 0x040013E7 RID: 5095
		private List<SlotGroup> allGroupsInOrder = new List<SlotGroup>();

		// Token: 0x040013E8 RID: 5096
		private SlotGroup[,,] groupGrid;

		// Token: 0x040013E9 RID: 5097
		[CompilerGenerated]
		private static Comparison<IHaulDestination> <>f__mg$cache0;

		// Token: 0x040013EA RID: 5098
		[CompilerGenerated]
		private static Comparison<SlotGroup> <>f__mg$cache1;

		// Token: 0x040013EB RID: 5099
		[CompilerGenerated]
		private static Comparison<IHaulDestination> <>f__mg$cache2;

		// Token: 0x040013EC RID: 5100
		[CompilerGenerated]
		private static Comparison<SlotGroup> <>f__mg$cache3;
	}
}