﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x020002F3 RID: 755
	public class Archive : IExposable
	{
		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x0006F0F4 File Offset: 0x0006D4F4
		public List<IArchivable> ArchivablesListForReading
		{
			get
			{
				return this.archivables;
			}
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0006F110 File Offset: 0x0006D510
		public void ExposeData()
		{
			Scribe_Collections.Look<IArchivable>(ref this.archivables, "archivables", LookMode.Deep, new object[0]);
			Scribe_Collections.Look<IArchivable>(ref this.pinnedArchivables, "pinnedArchivables", LookMode.Reference);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.archivables.RemoveAll((IArchivable x) => x == null);
				this.pinnedArchivables.RemoveWhere((IArchivable x) => x == null);
			}
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0006F1A8 File Offset: 0x0006D5A8
		public bool Add(IArchivable archivable)
		{
			bool result;
			if (archivable == null)
			{
				Log.Error("Tried to add null archivable.", false);
				result = false;
			}
			else if (this.Contains(archivable))
			{
				result = false;
			}
			else
			{
				this.archivables.Add(archivable);
				this.archivables.SortBy((IArchivable x) => x.CreatedTicksGame);
				this.CheckCullArchivables();
				result = true;
			}
			return result;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0006F224 File Offset: 0x0006D624
		public bool Remove(IArchivable archivable)
		{
			bool result;
			if (!this.Contains(archivable))
			{
				result = false;
			}
			else
			{
				this.archivables.Remove(archivable);
				this.pinnedArchivables.Remove(archivable);
				result = true;
			}
			return result;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0006F268 File Offset: 0x0006D668
		public bool Contains(IArchivable archivable)
		{
			return this.archivables.Contains(archivable);
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0006F289 File Offset: 0x0006D689
		public void Pin(IArchivable archivable)
		{
			if (this.Contains(archivable))
			{
				if (!this.IsPinned(archivable))
				{
					this.pinnedArchivables.Add(archivable);
				}
			}
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0006F2BB File Offset: 0x0006D6BB
		public void Unpin(IArchivable archivable)
		{
			if (this.Contains(archivable))
			{
				if (this.IsPinned(archivable))
				{
					this.pinnedArchivables.Remove(archivable);
				}
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0006F2F0 File Offset: 0x0006D6F0
		public bool IsPinned(IArchivable archivable)
		{
			return this.pinnedArchivables.Contains(archivable);
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x0006F314 File Offset: 0x0006D714
		private void CheckCullArchivables()
		{
			int num = 0;
			for (int i = 0; i < this.archivables.Count; i++)
			{
				if (!this.IsPinned(this.archivables[i]) && this.archivables[i].CanCullArchivedNow)
				{
					num++;
				}
			}
			int num2 = num - 200;
			for (int j = 0; j < this.archivables.Count; j++)
			{
				if (num2 <= 0)
				{
					break;
				}
				if (!this.IsPinned(this.archivables[j]) && this.archivables[j].CanCullArchivedNow)
				{
					if (this.Remove(this.archivables[j]))
					{
						num2--;
						j--;
					}
				}
			}
		}

		// Token: 0x04000830 RID: 2096
		private List<IArchivable> archivables = new List<IArchivable>();

		// Token: 0x04000831 RID: 2097
		private HashSet<IArchivable> pinnedArchivables = new HashSet<IArchivable>();

		// Token: 0x04000832 RID: 2098
		public const int MaxNonPinnedArchivables = 200;
	}
}